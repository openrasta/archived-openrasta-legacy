using System;
using System.Collections.Generic;
using System.Linq;
using OpenRasta.Web;
using OpenRasta.Web.UriDecorators;

namespace OpenRasta.Codecs.jsonp
{
    public class JsonPUriDecorator : IUriDecorator
    {
        readonly JsonPConfiguration _config;
        readonly ICommunicationContext _context;
        string _callback;
        

        public JsonPUriDecorator(JsonPConfiguration config, ICommunicationContext context)
        {
            _config = config;
            _context = context;
        }

        public bool Parse(Uri uri, out Uri processedUri)
        {
            _callback = GetCallback(uri);

            if (null == _callback)
            {
                processedUri = uri;
                return false;
            }

            var uriString = uri.ToString();
            processedUri = new Uri(uriString.Replace(_config.QueryString+"="+_callback, ""));
            return true;
        }

        public void Apply()
        {
            var entity = _context.Response.Entity;
            _context.PipelineData.ResponseCodec = new CodecRegistration(_config.CodecType, JsonPConfiguration.JsonPResourceKey, false, JsonPConfiguration.JsonPMediaType, new string[0], _callback, true);
            entity.ContentType = JsonPConfiguration.JsonPMediaType;
        }

        string GetCallback(Uri uri)
        {
            var qs = uri.Query.TrimStart('?');
            if (string.Empty == qs)
                return null;

            var values = from pair in qs.Split('&')
                         let kvp = pair.Split('=')
                         let key = kvp[0]
                         let value = kvp.Length == 2 ? kvp[1] : null
                         select new KeyValuePair<string, string>(key, value);

            string callback = null;
            if (values.Any(v => v.Key == _config.QueryString))
                callback = values.Where(v => v.Key == _config.QueryString).Single().Value;
            return callback;
        }

        public string QueryString { get; set; }
    }
}