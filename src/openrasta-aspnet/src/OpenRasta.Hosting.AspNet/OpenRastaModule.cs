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
using System.Threading;
using System.Web;
using System.Web.Hosting;
using OpenRasta.DI;
using OpenRasta.Diagnostics;
using OpenRasta.Pipeline;
using OpenRasta.Web;

namespace OpenRasta.Hosting.AspNet
{
    public class OpenRastaModule : IHttpModule
    {
        internal const string COMM_CONTEXT_KEY = "__OR_COMM_CONTEXT";
        internal const string ORIGINAL_PATH_KEY = "__ORIGINAL_PATH";
        internal const string SERVER_SOFTWARE_KEY = "SERVER_SOFTWARE_KEY";

        internal static HostManager HostManager;
        static readonly object _syncRoot = new object();

        static OpenRastaModule()
        {
            Host = new AspNetHost();
        }

        public static AspNetCommunicationContext CommunicationContext
        {
            get
            {
                var context = HttpContext.Current;
                if (context.Items.Contains(COMM_CONTEXT_KEY))
                    return (AspNetCommunicationContext)context.Items[COMM_CONTEXT_KEY];
                var orContext = new AspNetCommunicationContext(Log, 
                                                               context, 
                                                               new AspNetRequest(context), 
                                                               new AspNetResponse(context) { Log = Log });
                context.Items[COMM_CONTEXT_KEY] = orContext;
                return orContext;
            }
        }

        public static AspNetHost Host { get; private set; }
        public static Iis Iis { get; set; }

        protected static ILogger<AspNetLogSource> Log { get; set; }

        public void Dispose()
        {
            // we never unregister the host, as the AppDomain will die before we have to care.
        }

        public void Init(HttpApplication context)
        {
            context.PostResolveRequestCache += HandleHttpApplicationPostResolveRequestCacheEvent;
            context.EndRequest += HandleHttpApplicationEndRequestEvent;
            if (HostManager == null)
            {
                lock (_syncRoot)
                {
                    Thread.MemoryBarrier();
                    if (HostManager == null)
                    {
                        HostManager = HostManager.RegisterHost(Host);
                        Host.RaiseStart();
                        Log = HostManager.Resolver.Resolve<ILogger<AspNetLogSource>>();
                    }
                }
            }
        }

        static bool HandlerAlreadyMapped(string method, Uri path)
        {
            return Iis.IsHandlerAlreadyRegisteredForRequest(method, path);
        }

        static void VerifyIisDetected(HttpContext context)
        {
            lock (_syncRoot)
            {
                if (Iis == null)
                {
                    if (context == null)
                        throw new InvalidOperationException();
                    Iis iisVersion = null;
                    string serverSoftwareHeader = context.Request.ServerVariables[SERVER_SOFTWARE_KEY];

                    int slashPos = serverSoftwareHeader != null ? serverSoftwareHeader.IndexOf('/') : -1;
                    if (slashPos != -1)
                    {
                        string productName = serverSoftwareHeader.Substring(0, slashPos);
                        Version parsedVersion;
                        try
                        {
                            parsedVersion = new Version(serverSoftwareHeader.Substring(slashPos + 1, serverSoftwareHeader.Length - slashPos - 1).Trim());
                        }
                        catch
                        {
                            parsedVersion = null;
                        }

                        if (productName.EqualsOrdinalIgnoreCase("microsoft-iis") &&
                            parsedVersion != null &&
                            parsedVersion.Major >= 7)
                        {
                            iisVersion = new Iis7();
                        }
                    }

                    Iis = iisVersion ?? new Iis6();
                    Log.IisDetected(Iis, serverSoftwareHeader);
                }
            }
        }

        static void HandleHttpApplicationEndRequestEvent(object sender, EventArgs e)
        {
            if (HttpContext.Current.Items.Contains(ORIGINAL_PATH_KEY))
            {
                var commContext = (ICommunicationContext)((HttpApplication)sender).Context.Items[COMM_CONTEXT_KEY];

                Host.RaiseIncomingRequestProcessed(commContext);
            }
        }

        static void HandleHttpApplicationPostResolveRequestCacheEvent(object sender, EventArgs e)
        {
            VerifyIisDetected(HttpContext.Current);
            if (!HostingEnvironment.VirtualPathProvider.FileExists(HttpContext.Current.Request.Path)
                && (HttpContext.Current.Request.Path == "/" || !HostingEnvironment.VirtualPathProvider.DirectoryExists(HttpContext.Current.Request.Path))
                && !HandlerAlreadyMapped(HttpContext.Current.Request.HttpMethod, HttpContext.Current.Request.Url))
            {
                Log.StartPreExecution();
                var context = CommunicationContext;
                var stage = context.PipelineData.PipelineStage;
                if (stage == null)
                    context.PipelineData.PipelineStage = stage = new PipelineStage(HostManager.Resolver.Resolve<IPipeline>());
                stage.SuspendAfter<KnownStages.IUriMatching>();
                Host.RaiseIncomingRequestReceived(context);

                if (context.PipelineData.ResourceKey != null || context.OperationResult != null)
                {
                    HttpContext.Current.Items[ORIGINAL_PATH_KEY] = HttpContext.Current.Request.Path;
                    HttpContext.Current.RewritePath(VirtualPathUtility.ToAppRelative("~/ignoreme.rastahook"), false);
                    Log.PathRewrote();
                    return;
                }
                Log.IgnoredRequest();
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