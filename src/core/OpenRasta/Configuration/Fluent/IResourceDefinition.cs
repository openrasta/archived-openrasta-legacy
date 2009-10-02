namespace OpenRasta.Configuration.Fluent
{
    public interface IResourceDefinition
    {
        ICodecParentDefinition WithoutUri { get; }
        IUriDefinition AtUri(string uri);
    }
}