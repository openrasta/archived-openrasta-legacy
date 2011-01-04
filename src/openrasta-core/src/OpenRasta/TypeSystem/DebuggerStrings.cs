using System.Linq;

namespace OpenRasta.TypeSystem
{
    public static class DebuggerStrings
    {
        public static string Property(IProperty property)
        {
            if (property == null) return null;
            string propertyParams = string.Empty;
            if (property.PropertyParameters != null && property.PropertyParameters.Length > 0)
            {
                propertyParams = "[" + string.Join(", ", property.PropertyParameters.Select(x => x.ToString()).ToArray()) + "]";
            }

            return "{0} {1}.{2}".With(property.Type.Name, property.Owner.Type.Name, property.Name) + propertyParams;
        }
    }
}