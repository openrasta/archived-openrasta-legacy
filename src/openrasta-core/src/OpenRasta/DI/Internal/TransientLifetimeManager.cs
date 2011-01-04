namespace OpenRasta.DI.Internal
{
    internal class TransientLifetimeManager : DependencyLifetimeManager
    {
        public TransientLifetimeManager(InternalDependencyResolver builder)
            : base(builder)
        {
        }
    }
}