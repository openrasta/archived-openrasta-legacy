using System;
using System.Collections.Generic;
using OpenRasta.TypeSystem.ReflectionBased;

namespace OpenRasta.TypeSystem.Surrogates.Static
{
    public class CollectionSurrogateBuilder : AbstractStaticSurrogateBuilder
    {
        public override bool CanCreateFor(Type type)
        {
            return type.FindInterface(typeof(ICollection<>)) != null;
        }

        public override Type Create(Type type)
        {
            return typeof(CollectionIndexerSurrogate<>).MakeGenericType(
                type.FindInterface(typeof(ICollection<>)).GetGenericArguments()[0]);
        }
    }
}