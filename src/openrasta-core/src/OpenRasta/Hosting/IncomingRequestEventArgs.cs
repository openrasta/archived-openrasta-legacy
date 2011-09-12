using System;
using OpenRasta.Web;

namespace OpenRasta.Hosting
{
    public abstract class IncomingRequestEventArgs : EventArgs
    {
        public IncomingRequestEventArgs(ICommunicationContext context)
        {
            Context = context;
        }

        public ICommunicationContext Context { get; set; }
    }

    public class IncomingRequestProcessedEventArgs : IncomingRequestEventArgs
    {
        public IncomingRequestProcessedEventArgs(ICommunicationContext context)
            : base(context)
        {
        }
    }

    public class IncomingRequestReceivedEventArgs : IncomingRequestEventArgs
    {
        public IncomingRequestReceivedEventArgs(ICommunicationContext context)
            : base(context)
        {
        }
    }
}