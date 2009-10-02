namespace OpenRasta.Configuration.Fluent
{
    public interface ICodecWithMediaTypeDefinition : ICodecDefinition
    {
        ICodecWithMediaTypeDefinition ForExtension(string extension);
    }
}