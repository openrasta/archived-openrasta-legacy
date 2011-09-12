using System;
using System.Collections.Generic;
using OpenRasta.TypeSystem.ReflectionBased;

namespace OpenRasta.TypeSystem.Surrogates.Static
{
    public class ListIndexerSurrogateBuilder : AbstractStaticSurrogateBuilder
    {
        public override bool CanCreateFor(Type type)
        {
            var enumerable = type.FindInterface(typeof(IEnumerable<>));
            return type.FindInterface(typeof(IList<>)) != null
                   || enumerable == type;
        }

        public override Type Create(Type type)
        {
            return typeof(ListIndexerSurrogate<>).MakeGenericType(
                type.FindInterface(typeof(IEnumerable<>)).GetGenericArguments()[0]);
        }
    }
}