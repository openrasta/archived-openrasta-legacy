using System.IO;
using OpenRasta.IO;

namespace OpenRasta.Web
{
    public class HttpEntityFile : IFile
    {
        readonly IHttpEntity _entity;

        public HttpEntityFile(IHttpEntity entity)
        {
            _entity = entity;
        }

        public MediaType ContentType
        {
            get { return _entity.ContentType ?? MediaType.ApplicationOctetStream; }
        }

        public string FileName
        {
            get { return _entity.Headers.ContentDisposition != null ? _entity.Headers.ContentDisposition.FileName : null; }
        }

        public long Length
        {
            get { return _entity.Stream.Length; }
        }

        public Stream OpenStream()
        {
            return _entity.Stream;
        }
    }
}