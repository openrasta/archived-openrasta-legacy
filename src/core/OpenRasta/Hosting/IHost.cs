using System;
using OpenRasta.DI;

namespace OpenRasta.Hosting
{
    public interface IHost
    {
        string ApplicationVirtualPath { get; }
        event EventHandler Start;
        event EventHandler Stop;
        event EventHandler<IncomingRequestReceivedEventArgs> IncomingRequestReceived;
        event EventHandler<IncomingRequestProcessedEventArgs> IncomingRequestProcessed;
        bool ConfigureRootDependencies(IDependencyResolver resolver);
        bool ConfigureLeafDependencies(IDependencyResolver resolver);
        IDependencyResolverAccessor ResolverAccessor { get; }
    }
}