using OpenRasta.TypeSystem.Surrogated;
using OpenRasta.TypeSystem.Surrogates;
using OpenRasta.TypeSystem.Surrogates.Static;

namespace OpenRasta.TypeSystem.ReflectionBased
{
    public static class TypeSystems
    {
        static readonly ITypeSystem _default = new ReflectionBasedTypeSystem(
            new SurrogateBuilderProvider(
                new ISurrogateBuilder[]
                {
                    new DateTimeSurrogate(), 
                    new ListIndexerSurrogateBuilder(), 
                    new CollectionIndexerSurrogateBuilder()
                }), 
            new PathManager());

        public static ITypeSystem Default
        {
            get { return _default; }
        }
    }
}