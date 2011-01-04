using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OpenRasta.Binding;
using OpenRasta.Collections;
using OpenRasta.Web;

namespace OpenRasta.Codecs
{
    public static class CodecExtensions
    {
        public static MethodInfo GetReadKeyValuesMethod(this ICodec codec)
        {
            return codec.GetType().GetMethods().Select(m => new { method = m, parameters = m.GetParameters() }).
                Where(m =>
                      typeof(IEnumerable).IsAssignableFrom(m.method.ReturnType)
                      && m.parameters.Length == 1
                      && typeof(IHttpEntity).IsAssignableFrom(m.parameters[0].ParameterType))
                .Select(m => m.method)
                .First();
        }
        public static IEnumerable<KeyedValues> ReadKeyValues(this ICodec codec, IHttpEntity entity)
        {
            return ((IEnumerable)codec.GetReadKeyValuesMethod().Invoke(codec, new object[] { entity })).Cast<KeyedValues>();
        }

        public static bool TryAssignKeyValues(this ICodec codec, IHttpEntity entity, IObjectBinder binder)
        {
            return TryAssignKeyValues(codec, entity, binder, null, null);
        }

        public static bool TryAssignKeyValues(this ICodec codec, IHttpEntity entity, IObjectBinder binder, Action<KeyedValues> assigned, Action<KeyedValues> failed)
        {
            return codec.ReadKeyValues(entity)
                        .AsObservable(x => from kv in x
                                          where kv.SetProperty(binder)
                                          select kv, 
                                      assigned, 
                                      failed)
                        .Count() > 0;
        }

        public static bool TryAssignKeyValues(this ICodec codec, IHttpEntity entity, IEnumerable<IObjectBinder> binders)
        {
            return TryAssignKeyValues(codec, entity, binders, null, null);
        }

        public static bool TryAssignKeyValues(this ICodec codec, IHttpEntity entity, IEnumerable<IObjectBinder> binders, Action<KeyedValues> assigned, Action<KeyedValues> failed)
        {
            return binders.Aggregate(false, (result, binder) => codec.TryAssignKeyValues(entity, binder, assigned, failed) || result);
        }
        
    }
}