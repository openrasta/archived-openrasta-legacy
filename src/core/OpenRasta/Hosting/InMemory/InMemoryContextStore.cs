using OpenRasta.Collections;
using OpenRasta.Pipeline;

namespace OpenRasta.Hosting.InMemory
{
    public class InMemoryContextStore : NullBehaviorDictionary<string, object>, IContextStore
    {
    }
}