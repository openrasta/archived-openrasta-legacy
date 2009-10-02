#region License
/* Authors:
 *      Sebastien Lambla (seb@serialseb.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OpenRasta.Binding;
using OpenRasta.DI;
using OpenRasta.TypeSystem;
using OpenRasta.Web;

namespace OpenRasta.Codecs
{
    [MediaType("multipart/form-data;q=0.5")]
    [SupportedType(typeof(IEnumerable<IMultipartHttpEntity>))]
    [SupportedType(typeof(IDictionary<string, IList<IMultipartHttpEntity>>))]
    public class MultipartFormDataObjectCodec : AbstractMultipartFormDataCodec, IMediaTypeReader
    {
        public MultipartFormDataObjectCodec(ICommunicationContext context, ICodecRepository codecs, IDependencyResolver container, ITypeSystem typeSystem, IObjectBinderLocator binderLocator)
            : base(context, codecs, container, typeSystem, binderLocator)
        {
        }

        public object ReadFrom(IHttpEntity request, IType destinationType, string parameterName)
        {
            if (destinationType.IsAssignableFrom<IEnumerable<IMultipartHttpEntity>>())
            {
                var multipartReader = new MultipartReader(request.ContentType.Boundary, request.Stream);
                return multipartReader.GetParts();
            }
            if (destinationType.IsAssignableFrom<IDictionary<string, IList<IMultipartHttpEntity>>>())
            {
                return FormData(request);
            }
            var binder = BinderLocator.GetBinder(destinationType);
            if (binder == null)
                throw new InvalidOperationException("Cannot find a binder to create the object");
            binder.Prefixes.Add(parameterName);
            bool wasAnyKeyUsed = ReadKeyValues(request).Aggregate(false, (wasUsed, kv) => kv.SetProperty(binder) || wasUsed);
            var result = binder.BuildObject();

            return wasAnyKeyUsed && result.Successful ? result.Instance : Missing.Value;
        }
    }
}

#region Full license
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion