namespace OpenRasta.Configuration.Fluent
{
    public interface IResourceDefinition : INoIzObject
    {
        ICodecParentDefinition WithoutUri { get; }
        IUriDefinition AtUri(string uri);
    }
}