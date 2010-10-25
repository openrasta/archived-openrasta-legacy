using OpenRasta.Web;

namespace OpenRasta.Configuration.Fluent
{
    public interface ICodecDefinition : INoIzObject, IRepeatableDefinition<ICodecParentDefinition>
    {
        ICodecWithMediaTypeDefinition ForMediaType(MediaType mediaType);
    }
}