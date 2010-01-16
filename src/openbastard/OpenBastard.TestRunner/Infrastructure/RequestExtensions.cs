using System.IO;
using System.Linq;
using System.Text;
using OpenRasta;
using OpenRasta.IO;
using OpenRasta.Security;
using OpenRasta.Web;

namespace OpenBastard.Infrastructure
{
    public static class RequestExtensions
    {
        public static TRequest Accept<TRequest>(this TRequest request, params MediaType[] acceptedMediaTypes) where TRequest:IRequest
        {
            return request.Accept(acceptedMediaTypes.Select(x => x.ToString()).ToArray());
        }

        public static TRequest Accept<TRequest>(this TRequest request, params string[] acceptedMediaTypes) where TRequest : IRequest
        {
            request.Headers["Accept"] = string.Join(",", acceptedMediaTypes);
            return request;
        }

        public static TRequest ContentLength<TRequest>(this TRequest request, long length) where TRequest : IRequest
        {
            request.Entity.ContentLength = length;
            return request;
        }

        public static TRequest ContentType<TRequest>(this TRequest request, string contentType) where TRequest : IRequest
        {
            request.Entity.ContentType = new MediaType(contentType);
            return request;
        }

        public static TRequest Delete<TRequest>(this TRequest request) where TRequest : IRequest
        {
            request.HttpMethod = "DELETE";
            return request;
        }
        public static TRequest Credentials<TRequest>(this TRequest request, string username, string password) where TRequest:IClientRequest
        {
            request.Credentials = new Credentials { Username = username, Password = password };
            return request;
        }
        public static TRequest EntityAsMultipartFormData<TRequest>(this TRequest request, params FormData[] entities) where TRequest : IRequest
        {
            string boundary = "mordor";

            var memoryStream = new MemoryStream();

            for (int i = 0; i < entities.Length; i++)
            {
                memoryStream.Write(entities[i].Serialize(boundary));
            }
            memoryStream.Write(Encoding.ASCII.GetBytes("\r\n--{0}--\r\n".With(boundary)));
            memoryStream.Position = 0;

            return request
                .ContentType("multipart/form-data;boundary=" + boundary)
                .ContentLength(memoryStream.Length)
                .EntityBody(memoryStream.ReadToEnd());
        }

        public static TRequest EntityBody<TRequest>(this TRequest request, byte[] bytesToWrite) where TRequest : IRequest
        {
            new MemoryStream(bytesToWrite).CopyTo(request.Entity.Stream);
            return request;
        }

        public static TRequest Get<TRequest>(this TRequest request) where TRequest : IRequest
        {
            request.HttpMethod = "GET";
            return request;
        }

        public static TRequest Method<TRequest>(this TRequest request, string httpMethod) where TRequest : IRequest
        {
            request.HttpMethod = httpMethod;
            return request;
        }

        public static TRequest Post<TRequest>(this TRequest request) where TRequest : IRequest
        {
            request.HttpMethod = "POST";
            return request;
        }

        public static TRequest Put<TRequest>(this TRequest request) where TRequest : IRequest
        {
            request.HttpMethod = "PUT";
            return request;
        }
    }

    public class FormData
    {
        readonly byte[] _content;
        readonly string _contentDisposition;

        FormData(string contentDisposition, byte[] content)
        {
            _contentDisposition = contentDisposition;
            _content = content;
        }

        public static FormData File(string fieldName, string fileName, byte[] fileData, string mediaType)
        {
            return new FormData(("form-data;filename=\"{0}\";name=\"{1}\"\r\nContent-Type: " + mediaType).With(fileName, fieldName), fileData);
        }

        public static FormData Text(string fieldName, string fieldValue)
        {
            return new FormData("form-data;name=\"{0}\"".With(fieldName), Encoding.UTF8.GetBytes(fieldValue));
        }

        public byte[] Serialize(string boundary)
        {
            return Encoding.ASCII.GetBytes("\r\n--{0}\r\nContent-Disposition: {1}\r\n\r\n".With(boundary, _contentDisposition))
                .Concat(_content)
                .ToArray();
        }
    }
}