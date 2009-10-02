using System.Collections.Generic;
using System.Linq;
using OpenRasta.TypeSystem;

namespace OpenRasta.Configuration.MetaModel
{
    public class ResourceModel
    {
        public ResourceModel()
        {
            Uris = new List<UriModel>();
            Handlers = new List<IType>();
            Codecs = new List<CodecModel>();
        }

        public IList<CodecModel> Codecs { get; private set; }
        public IList<IType> Handlers { get; private set; }

        public bool IsStrictRegistration { get; set; }
        public object ResourceKey { get; set; }
        public IList<UriModel> Uris { get; private set; }

        public override string ToString()
        {
            return string.Format("ResourceKey: {0}, Uris: {1}, Handlers: {2}, Codecs: {3}", 
                                 ResourceKey, 
                                 Uris.Aggregate(string.Empty, (str, reg) => str + reg + ";"), 
                                 Handlers.Aggregate(string.Empty, (str, reg) => str + reg + ";"), 
                                 Codecs.Aggregate(string.Empty, (str, reg) => str + reg + ";"));
        }
    }
}