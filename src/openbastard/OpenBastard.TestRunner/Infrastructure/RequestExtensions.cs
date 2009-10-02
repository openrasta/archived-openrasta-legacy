using System.IO;
using System.Linq;
using System.Text;
using OpenRasta;
using OpenRasta.IO;
using OpenRasta.Web;

namespace OpenBastard.Infrastructure
{
    public static class RequestExtensions
    {
        public static IRequest Accept(this IRequest request, params MediaType[] acceptedMediaTypes)
        {
            return request.Accept(acceptedMediaTypes.Select(x => x.ToString()).ToArray());
        }

        public static IRequest Accept(this IRequest request, params string[] acceptedMediaTypes)
        {
            request.Headers["Accept"] = string.Join(",", acceptedMediaTypes);
            return request;
        }

        public static IRequest ContentLength(this IRequest request, long length)
        {
            request.Entity.ContentLength = length;
            return request;
        }

        public static IRequest ContentType(this IRequest request, string contentType)
        {
            request.Entity.ContentType = new MediaType(contentType);
            return request;
        }

        public static IRequest Delete(this IRequest request)
        {
            request.HttpMethod = "DELETE";
            return request;
        }

        public static IRequest EntityAsMultipartFormData(this IRequest request, params FormData[] entities)
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

        public static IRequest EntityBody(this IRequest request, byte[] bytesToWrite)
        {
            new MemoryStream(bytesToWrite).CopyTo(request.Entity.Stream);
            return request;
        }

        public static IRequest Get(this IRequest request)
        {
            request.HttpMethod = "GET";
            return request;
        }

        public static IRequest Method(this IRequest request, string httpMethod)
        {
            request.HttpMethod = httpMethod;
            return request;
        }

        public static IRequest Post(this IRequest request)
        {
            request.HttpMethod = "POST";
            return request;
        }

        public static IRequest Put(this IRequest request)
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