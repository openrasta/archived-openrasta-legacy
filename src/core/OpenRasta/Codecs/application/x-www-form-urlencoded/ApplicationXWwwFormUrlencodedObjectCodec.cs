using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OpenRasta.Binding;
using OpenRasta.TypeSystem;
using OpenRasta.Web;

namespace OpenRasta.Codecs
{
    [MediaType("application/x-www-form-urlencoded;q=0.5")]
    [SupportedType(typeof(IDictionary<string, string[]>))]
    [SupportedType(typeof(Dictionary<string, string[]>))]
    public class ApplicationXWwwFormUrlencodedObjectCodec : AbstractApplicationXWwwFormUrlencodedCodec, IMediaTypeReader
    {
        public ApplicationXWwwFormUrlencodedObjectCodec(ICommunicationContext context, IObjectBinderLocator locator)
            : base(context, locator)
        {
        }

        public object ReadFrom(IHttpEntity request, IType destinationType, string destinationName)
        {
            if (IsRawDictionary(destinationType))
                return FormData(request);
            
            var binder = _binderLocator.GetBinder(destinationType);
            if (binder == null)
                throw new InvalidOperationException("Cannot find a binder to create the object");
            binder.Prefixes.Add(destinationName);
            bool wasAnyKeyUsed = ReadKeyValues(request).Aggregate(false, (wasUsed, kv) => kv.SetProperty(binder) || wasUsed);
            var result = binder.BuildObject();

            return wasAnyKeyUsed && result.Successful ? result.Instance : Missing.Value;
        }
    }
}