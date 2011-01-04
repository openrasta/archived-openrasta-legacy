namespace OpenRasta.TypeSystem
{
    public class PathComponent
    {
        public PathComponent()
        {
            ParsedValue = string.Empty;
            Type = PathComponentType.None;
        }
        public PathComponentType Type { get; set; }
        public string ParsedValue { get; set; }
    }
}