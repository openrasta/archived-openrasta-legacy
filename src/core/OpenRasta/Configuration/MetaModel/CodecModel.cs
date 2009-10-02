using System;
using System.Collections.Generic;
using OpenRasta.Codecs;
using OpenRasta.TypeSystem.ReflectionBased;

namespace OpenRasta.Configuration.MetaModel
{
    public class CodecModel
    {
        public CodecModel(Type codecType) : this(codecType, null)
        {
        }

        public CodecModel(Type codecType, object configuration)
        {
            if (codecType == null)
                throw new ArgumentNullException("codecType");
            if (!codecType.IsAssignableTo<ICodec>())
                throw new ArgumentOutOfRangeException("codecType", "The type {0} doesn't implement ICodec".With(codecType.FullName));

            CodecType = codecType;
            Configuration = configuration;
            MediaTypes = new List<MediaTypeModel>();
        }

        public Type CodecType { get; set; }

        public object Configuration { get; set; }
        public IList<MediaTypeModel> MediaTypes { get; private set; }
    }
}