using System;
using System.Runtime.Remoting.Messaging;

namespace OpenRasta.Hosting
{
    public class ContextScope : IDisposable
    {
        readonly object _hostContext;
        readonly object _savedHostContext;

        public ContextScope(object context)
        {
            _savedHostContext = CallContext.HostContext;
            _hostContext = CallContext.HostContext = context;
        }

        public void Dispose()
        {
            if (_hostContext != _savedHostContext)
            {
                CallContext.HostContext = _savedHostContext;
            }
        }
    }
}