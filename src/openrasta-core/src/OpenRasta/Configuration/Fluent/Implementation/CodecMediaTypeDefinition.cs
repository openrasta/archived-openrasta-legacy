using OpenRasta.Configuration.MetaModel;
using OpenRasta.Web;

namespace OpenRasta.Configuration.Fluent.Implementation
{
    public class CodecMediaTypeDefinition : ICodecWithMediaTypeDefinition
    {
        readonly MediaTypeModel _model;
        readonly CodecDefinition _parent;

        public CodecMediaTypeDefinition(CodecDefinition parent, MediaTypeModel model)
        {
            _parent = parent;
            _model = model;
        }

        public ICodecParentDefinition And
        {
            get { return _parent.And; }
        }

        public ICodecWithMediaTypeDefinition ForMediaType(MediaType mediaType)
        {
            return _parent.ForMediaType(mediaType);
        }

        public ICodecWithMediaTypeDefinition ForExtension(string extension)
        {
            _model.Extensions.Add(extension);
            return this;
        }
    }
}