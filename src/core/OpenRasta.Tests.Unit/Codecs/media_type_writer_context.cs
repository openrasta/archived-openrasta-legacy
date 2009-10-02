using OpenRasta.Web;

namespace OpenRasta.Codecs
{
    public abstract class media_type_writer_context<TCodec> : codec_context<TCodec> where TCodec : ICodec
    {
        protected IResponse Response { get { return base.Context.Response; } }
    }
}