using OpenRasta.Binding;
using OpenRasta.DI;
using OpenRasta.TypeSystem;
using OpenRasta.Web;

namespace OpenRasta.Codecs
{
    [MediaType("multipart/form-data;q=0.5")]
    [SupportedType(typeof(object))]
    public class MultipartFormDataKeyedValuesCodec : AbstractMultipartFormDataCodec, IKeyedValuesMediaTypeReader<IMultipartHttpEntity>
    {
        public MultipartFormDataKeyedValuesCodec(ICommunicationContext context, ICodecRepository codecs, IDependencyResolver container, ITypeSystem typeSystem, IObjectBinderLocator binderLocator)
            : base(context, codecs, container, typeSystem, binderLocator)
        {
        }
    }
}