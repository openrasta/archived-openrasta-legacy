#region License
/* Authors:
 *      Sebastien Lambla (seb@serialseb.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */
#endregion
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization.Json;
using OpenRasta.Binding;
using OpenRasta.Codecs;
using OpenRasta.TypeSystem;
using OpenRasta.Web;

namespace OpenRasta.Codecs
{
    [MediaType("application/json;q=0.5", "json")]
    public class JsonDataContractCodec : IMediaTypeReader, IMediaTypeWriter
    {
        public object Configuration { get; set; }

        public object ReadFrom(IHttpEntity request, IType destinationType, string paramName)
        {
            if (destinationType is INativeMember)
                return new DataContractJsonSerializer(((INativeMember)destinationType).NativeType).ReadObject(request.Stream);
            return Missing.Value;
        }

        public void WriteTo(object entity, IHttpEntity response, string[] paramneters)
        {
            if (entity == null)
                return;
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(entity.GetType());
            serializer.WriteObject(response.Stream, entity);
        }
    }
}

#region Full license
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion