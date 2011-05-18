using System;
using System.Text;
using System.Text.RegularExpressions;
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

        public JsonPCodec(string handlerName, TCodec underlyingCodec)
            : this(underlyingCodec)
        {
            Configuration = handlerName;
        }

        public object Configuration
        {
            get; set;
        }

        public void WriteTo(object entity, IHttpEntity response, string[] codecParameters)
        {
            var handler = Configuration.ToString();
            ValidateCallback(handler);
            var handlerCallBytes = Encoding.UTF8.GetBytes(handler+"(");
            var handlerEndBytes = Encoding.UTF8.GetBytes(");");

            response.Stream.Write(handlerCallBytes, 0, handlerCallBytes.Length);
            _underlyingJsonCodec.WriteTo(entity, response, codecParameters);
            response.Stream.Write(handlerEndBytes, 0, handlerEndBytes.Length);
        }

        void ValidateCallback(string handler)
        {
            var expression = new Regex(@"                            # The following forms are whitelisted:
                ^[A-Za-z0-9_$]+$ |                                     # A plain identifier, eg. foo or my_function
                ^[A-Za-z0-9_$]+ \. [A-Za-z0-9_$]+$  |                  # Two identifiers separated by exactly one period, eg $my_object.DoThings
                ^[A-Za-z0-9_$]+ \[ ['""] [A-Za-z0-9_$]+ ['""] \] $ |   # An identifier followed by a second quoted identifier in brackets eg myfunctions['awesome_func'].
                ^[A-Za-z0-9_$]+ \[ [0-9]+ \]$                          # An identifier followed by an integer array access eg myfunctions[0]
            ", RegexOptions.IgnorePatternWhitespace);

            if(false == expression.IsMatch(handler))
                throw new InvalidJsonCallbackException();
        }


    }

    public class InvalidJsonCallbackException : Exception
    {
    }
}