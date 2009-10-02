using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenRasta.Web;

namespace OpenRasta.Codecs
{
    [MediaType("application/xhtml+xml;q=0.9")]
    [MediaType("text/html")]
    [SupportedType(typeof(OperationResult))]
    public class OperationResultCodec : Codec, IMediaTypeWriter
    {
        public void WriteTo(object entity, IHttpEntity response, string[] codecParameters)
        {
            
        }
    }
}
