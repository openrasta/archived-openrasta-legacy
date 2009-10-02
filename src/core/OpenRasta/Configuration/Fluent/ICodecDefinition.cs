using OpenRasta.Web;

namespace OpenRasta.Configuration.Fluent
{
    public interface ICodecDefinition : IRepeatableDefinition<ICodecParentDefinition>
    {
        ICodecWithMediaTypeDefinition ForMediaType(MediaType mediaType);
    }
}