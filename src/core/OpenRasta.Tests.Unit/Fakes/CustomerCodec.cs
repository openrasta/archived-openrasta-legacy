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
using OpenRasta.Binding;
using OpenRasta.Codecs;
using OpenRasta.TypeSystem;
using OpenRasta.Web;

namespace OpenRasta.Tests.Unit.Fakes
{
    public class CustomerCodec : Codec, IMediaTypeReader, IMediaTypeWriter
    {
        public object ReadFrom(IHttpEntity request, IType type, string paramName) { return type.CreateInstance(); }

        public void WriteTo(object entity, IHttpEntity response, string[] p) { response.Headers["ENTITY_TYPE"] = entity.GetType().Name; }
    }
    public class AnotherCustomerCodec : CustomerCodec
    {
    }

    public class KeyValuesCustomerCodec : Codec, IKeyedValuesMediaTypeReader<string>, IMediaTypeReader
    {
        
        public IEnumerable<KeyedValues<string>> ReadKeyValues(IHttpEntity entity) {
            yield return new KeyedValues<string>("FirstName", new[] { "John" }, Converter);
            yield return new KeyedValues<string>("LastName", new[] { "Doe" }, Converter);
            yield return new KeyedValues<string>("Username", new[] { "johndoe" }, Converter);
        }

        static BindingResult Converter(string t1, Type t2) { return BindingResult.Success(t1); }
        public object ReadFrom(IHttpEntity request, IType destinationType, string destinationName)
        {
            return new Customer {FirstName = "Jean", LastName = "Dupont", Username = "jeandupont"};
        }
    }
    internal class KeyValuesCustomerAndAddressCodec : Codec, IKeyedValuesMediaTypeReader<string>, IMediaTypeReader
    {

        public IEnumerable<KeyedValues<string>> ReadKeyValues(IHttpEntity entity)
        {
            yield return new KeyedValues<string>("Customer.FirstName", new[] { "John" }, Converter);
            yield return new KeyedValues<string>("Customer.LastName", new[] { "Doe" }, Converter);
            yield return new KeyedValues<string>("Customer.Username", new[] { "johndoe" }, Converter);
            yield return new KeyedValues<string>("Address.City", new[] { "London" }, Converter);
        }

        static BindingResult Converter(string t1, Type t2) { return BindingResult.Success(t1); }
        public object ReadFrom(IHttpEntity request, IType destinationType, string destinationName)
        {
            return null;
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