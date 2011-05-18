using System.Text;
using OpenRasta.Web;

namespace OpenRasta.Codecs.jsonp
{
    public class JsonPCodec<TCodec> : IMediaTypeWriter
        where TCodec : IMediaTypeWriter
    {
        readonly IMediaTypeWriter _underlyingJsonCodec;

        public JsonPCodec(TCodec underlyingJsonCodec)
        {
            _underlyingJsonCodec = underlyingJsonCodec;
        }

        public object Configuration
        {
            get; set;
        }

        public void WriteTo(object entity, IHttpEntity response, string[] codecParameters)
        {
            var handler = Configuration.ToString();
            var handlerCallBytes = Encoding.UTF8.GetBytes(handler+"(");
            var handlerEndBytes = Encoding.UTF8.GetBytes(");");

            response.Stream.Write(handlerCallBytes, 0, handlerCallBytes.Length);
            _underlyingJsonCodec.WriteTo(entity, response, codecParameters);
            response.Stream.Write(handlerEndBytes, 0, handlerEndBytes.Length);
        }            
    }
}