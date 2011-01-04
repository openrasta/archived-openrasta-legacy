using System;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using OpenRasta.Configuration;

namespace OpenRasta.Hosting.AspNet.AspNetHttpListener
{
// Warning, this class will undergo massive refactorings sooner or later, don't rely on it.
    public class HttpListenerAspNetHost : MarshalByRefObject
    {
        System.Net.HttpListener _listener;
        string _physicalDir;
        string _virtualDir;

        public void Configure(string[] prefixes, string vdir, string pdir)
        {
            _virtualDir = vdir;
            _physicalDir = pdir;
            _listener = new System.Net.HttpListener();
            foreach (string prefix in prefixes)
                _listener.Prefixes.Add(prefix);
        }

        public void ExecuteConfig(Action t)
        {
            ((Config)OpenRastaModule.Host.ConfigurationSource).ConfigurationLambda = () =>
                {
                    using (OpenRastaConfiguration.Manual)
                    {
                        t();
                    }
                };
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void ProcessRequest()
        {
            HttpListenerContext ctx;
            try
            {
                ctx = _listener.GetContext();
            }
            catch (HttpListenerException)
            {
                return;
            }
            QueueNextRequestWait();
            var workerRequest = new HttpListenerWorkerRequest(ctx, _virtualDir, _physicalDir);
            try
            {
                HttpRuntime.ProcessRequest(workerRequest);
            }
            catch
            {
            }
        }

        public void Start()
        {
            OpenRastaModule.Host.ConfigurationSource = new Config();
            _listener.Start();
            QueueNextRequestWait();
        }

        public void Stop()
        {
            _listener.Stop();
            ApplicationManager.GetApplicationManager().ShutdownAll();
        }

        void QueueNextRequestWait()
        {
            ThreadPool.QueueUserWorkItem(s => ProcessRequest());
        }

        class Config : IConfigurationSource
        {
            public Action ConfigurationLambda;

            public void Configure()
            {
                ConfigurationLambda();
            }
        }
    }
}