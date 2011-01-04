using System.Collections.Generic;
using OpenRasta.Pipeline;

namespace OpenRasta.DI.Internal
{
    public static class ContextStoreExtensions
    {
        const string CTX_INSTANCES_KEY = "__OR_CTX_INSTANCES_KEY";

        public static void Destruct(this IContextStore store)
        {
            foreach (var dep in store.GetContextInstances())
                if (dep.Cleaner != null)
                    dep.Cleaner.Destruct(dep.Key, dep.Instance);
            store.GetContextInstances().Clear();
        }

        public static IList<ContextStoreDependency> GetContextInstances(this IContextStore store)
        {
            return (IList<ContextStoreDependency>)
                   (store[CTX_INSTANCES_KEY] ?? (store[CTX_INSTANCES_KEY] = new List<ContextStoreDependency>()));
        }
    }
}