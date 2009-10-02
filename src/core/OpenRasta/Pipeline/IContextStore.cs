namespace OpenRasta.Pipeline
{
    public interface IContextStore
    {
        object this[string key] { get; set; }
    }
}