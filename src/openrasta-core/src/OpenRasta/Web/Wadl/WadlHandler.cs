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
using System.Text;
using OpenRasta.Handlers;
using OpenRasta.Configuration;
using OpenRasta.DI;

namespace OpenRasta.Web.Configuration.Wadl
{
    public class WadlHandler
    {
        readonly IDependencyResolver _resolver;
        private IUriResolver _uriRepository;
        private IHandlerRepository _handlerRepository;
        public WadlHandler(IDependencyResolver resolver, IUriResolver uriRepository, IHandlerRepository handlerRepository)
        {
            _resolver = resolver;
            _uriRepository = uriRepository;
            _handlerRepository = handlerRepository;
        }
        public WadlApplication Get()
        {
            var templateProcessor = _uriRepository as IUriTemplateParser;
            if (templateProcessor == null)
                throw new InvalidOperationException("The system doesn't have a IUriTemplateParser, WADL generation cannot proceed.");

            var app = new WadlApplication
                          {
                              Resources =
                                  {
                                      BasePath = _resolver.Resolve<ICommunicationContext>().ApplicationBaseUri.ToString()
                                  }
                          };

            foreach (var uriMap in _uriRepository)
            {
                var resource = new WadlResource { Path = uriMap.UriTemplate };

                var templateParameters = templateProcessor.GetTemplateParameterNamesFor(uriMap.UriTemplate);
                var queryParameters = templateProcessor.GetQueryParameterNamesFor(uriMap.UriTemplate);

                
                resource.Parameters = new System.Collections.ObjectModel.Collection<WadlResourceParameter>();
                foreach (string parameter in templateParameters)
                    resource.Parameters.Add(new WadlResourceParameter { Style= WadlResourceParameterStyle.Template, Name = parameter });

                foreach (string parameter in queryParameters)
                    resource.Parameters.Add(new WadlResourceParameter { Style = WadlResourceParameterStyle.Query, Name = parameter });

                // TODO: For each parameter, lookup the parameter type from the matched handler and include the xsd type in it

                app.Resources.Add(resource);
            }
            return app;
        }
    }
}

#region Full license
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion
