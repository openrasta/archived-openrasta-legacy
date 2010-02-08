using System.Linq;

namespace OpenRasta.TypeSystem
{
    public static class PropertyExtensions
    {
        public static IProperty FindPropertyByPath(this IMember source, string propertyName)
        {
            var sourceMember = source;
            IProperty property = null;
            foreach (var parseResult in source.TypeSystem.PathManager.ReadComponents(propertyName).Where(x => x.Type != PathComponentType.None))
            {
                if (parseResult.Type == PathComponentType.Indexer)
                    property = sourceMember.GetIndexer(parseResult.ParsedValue);
                else if (parseResult.Type == PathComponentType.Member)
                    property = sourceMember.GetProperty(parseResult.ParsedValue);

                if (property == null)
                    return null;

                sourceMember = property = property.TypeSystem.SurrogateProvider != null
                                              ? property.TypeSystem.SurrogateProvider.FindSurrogate(property)
                                              : property;
            }

            return property;
        }
    }
}