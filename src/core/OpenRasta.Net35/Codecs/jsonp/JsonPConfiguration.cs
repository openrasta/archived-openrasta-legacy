using System;
using OpenRasta.Web;

namespace OpenRasta.Codecs.jsonp
{
    public interface IJsonPConfiguration
    {
        string QueryString { get; set; }
        Type CodecType { get; set; }
    }

    public class JsonPConfiguration : IJsonPConfiguration
    {
        public static readonly MediaType JsonPMediaType = new MediaType("application/json-p");
        public const string JsonPResourceKey = "openrasta::internal::jsonp";

        public string QueryString { get; set; }
        public Type CodecType { get; set; }
    }
}