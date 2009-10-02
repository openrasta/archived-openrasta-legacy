#region License
/* Authors:
 *      Sebastien Lambla (seb@serialseb.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Compilation;
using System.Web.UI;
using OpenRasta.Collections.Specialized;
using OpenRasta.DI;
using OpenRasta.Diagnostics;
using OpenRasta.IO;
using OpenRasta.Web;

namespace OpenRasta.Codecs.WebForms
{
    [MediaType("application/xhtml+xml;q=0.9", "xhtml")]
    [MediaType("text/html", "html")]
    [MediaType("application/vnd.openrasta.htmlfragment+xml;q=0.5")]
    public class WebFormsCodec : IMediaTypeWriter
    {
        static readonly string[] DEFAULT_VIEW_NAMES = new[] { "index", "default", "view", "get" };
        readonly IRequest _request;

        public WebFormsCodec(IRequest request)
        {
            _request = request;
        }

        public IDictionary<string, string> Configuration { get; private set; }
        public ILogger Log { get; set; }

        object ICodec.Configuration
        {
            get
            {
                return Configuration;
            }
            set
            {
                if (value != null)
                    Configuration = value.ToCaseInvariantDictionary();
            }
        }

        public static void FlowResource(object sender, EventArgs e)
        {
            var page = sender as Page;
            if (page == null)
                return;
        }

        public static string GetViewVPath(IDictionary<string, string> codecConfiguration, string[] codecUriParameters, string uriName)
        {
            // if no pages were defined, return 501 not implemented
            if (codecConfiguration == null || codecConfiguration.Count == 0)
            {
                return null;
            }

            // if no codec parameters in the uri, take the default or return null
            if (codecUriParameters == null || codecUriParameters.Length == 0)
            {
                if (uriName != null && codecConfiguration.ContainsKey(uriName))
                    return codecConfiguration[uriName];
                return GetDefaultVPath(codecConfiguration);
            }


// if there's a codec parameter, take the first one and try to return the view if it exists
            string requestParameter = codecUriParameters[codecUriParameters.Length - 1];
            if (codecConfiguration.Keys.Contains(requestParameter))
                return codecConfiguration[requestParameter];

            // if theres a codec parameter and a uri name that doesn't match it, return teh default
            if (!uriName.IsNullOrEmpty())
                return GetDefaultVPath(codecConfiguration);

            return null;
        }

        public void WriteTo(object entity, IHttpEntity response, string[] codecParameters)
        {
            // The default webforms renderer only associate the last parameter in the codecParameters
            // with a page that has been defined in the rendererParameters.
            object renderTarget = null;
            if (entity != null && entity is Page)
                renderTarget = entity as Page;
            var codecParameterList = new List<string>(codecParameters);
            if (!string.IsNullOrEmpty(_request.UriName))
                codecParameterList.Add(_request.UriName);
            if (renderTarget == null)
            {
                string templateAddress = GetViewVPath(Configuration, codecParameterList.ToArray(), _request.UriName);

                var type = BuildManager.GetCompiledType(templateAddress);

                renderTarget = DependencyManager.GetService(type);

                if (entity != null)
                {
                    var page = renderTarget as Page;
                    if (page != null)
                    {
                        page.Init += (s, e) =>
                            {
                                SetAnyPropertyOfCorrectType(page, entity);
                                SetAnyPropertyOfCorrectType(page, response.Errors);
                                var master = page.Master;
                                while (master != null)
                                {
                                    SetAnyPropertyOfCorrectType(master, entity);
                                    SetAnyPropertyOfCorrectType(master, response.Errors);
                                    master = master.Master;
                                }
                            };
                    }
                    else
                    {
                        SetAnyPropertyOfCorrectType(renderTarget, entity);
                        SetAnyPropertyOfCorrectType(renderTarget, response.Errors);
                    }
                }
            }
            if (renderTarget != null)
                RenderTarget(response, renderTarget);
            else
                throw new InvalidOperationException("No http handler has been found to render the resource.");
        }

        static string GetDefaultVPath(IDictionary<string, string> codecConfiguration)
        {
            foreach (string defaultViewName in DEFAULT_VIEW_NAMES)
                if (codecConfiguration.Keys.Contains(defaultViewName))
                    return codecConfiguration[defaultViewName];
            return null;
        }

        static void SetAnyPropertyOfCorrectType(object propertyReceiver, object propertyValue)
        {
            if (propertyReceiver == null || propertyValue == null) return;
            var receiverType = propertyReceiver.GetType();

            var propertyType = propertyValue.GetType();

            var pis =
                receiverType.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo selectedProperty = null;
            foreach (var pi in pis)
            {
                if (pi.PropertyType.IsAssignableFrom(propertyValue.GetType()))

                    selectedProperty = pi;
            }
            if (selectedProperty != null && selectedProperty.CanWrite && propertyType.IsAssignableFrom(propertyValue.GetType()))
                selectedProperty.SetValue(propertyReceiver, propertyValue, null);
        }

        void RenderTarget(IHttpEntity response, object target)
        {
            var httpHandler = target as IHttpHandler;

            var targetEncoding = Encoding.UTF8;
            response.ContentType.CharSet = targetEncoding.HeaderName;
            TextWriter writer = null;
            var isDisposable = target as IDisposable;
            bool ownsWriter = false;
            try
            {
                if (response is ISupportsTextWriter)
                {
                    writer = ((ISupportsTextWriter)response).TextWriter;
                }
                else
                {
                    writer = new DeterministicStreamWriter(response.Stream, targetEncoding, StreamActionOnDispose.None);
                    ownsWriter = true;
                }
                if (target is UserControl)
                {
                    var dummy = new Page();
                    dummy.Controls.Add(target as UserControl);
                    httpHandler = dummy;
                }
                if (httpHandler != null)
                    HttpContext.Current.Server.Execute(httpHandler, writer, false);
            }
            finally
            {
                if (isDisposable != null)
                    isDisposable.Dispose();
                if (ownsWriter)
                    writer.Dispose();
            }
        }
    }
}

#region Full license
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion