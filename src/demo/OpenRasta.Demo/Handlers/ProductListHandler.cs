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
using OpenRasta.Demo.Resources;
using OpenRasta.Web;

namespace OpenRasta.Demo.Handlers
{
    public class ProductListHandler
    {
        internal static List<Product> Products = new List<Product>();

        static ProductListHandler()
        {
            Products.AddRange(new[] {
                new Product {Name = "testdriven.net", Description = "Test driven add-in from Jamie Cansdale"},
                new Product {Name = "moq", Description = "Lambda-driven mocking framework"},
                new Product {Name = "Castle Windsor", Description = "OSS DI and MVC"},
                new Product {Name = "Visual Studio", Description = "Coding monolith"} });        
        }

        public OperationResult Get()
        {
            return new OperationResult.OK {ResponseResource = Products};
        }

        public OperationResult Post(Product newProduct)
        {
            Products.Add(newProduct);
            return new OperationResult.SeeOther {RedirectLocation = typeof (IList<Product>).CreateUri()};
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