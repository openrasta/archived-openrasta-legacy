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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using OpenRasta.Collections;
using OpenRasta.Web.Markup.Attributes;
using OpenRasta.Web.Markup.Elements;
using OpenRasta.Web.Markup.Modules;

namespace OpenRasta.Web.Markup
{
    public class Document
    {
        public static T CreateElement<T>()
            where T : class, IElement
        {
            string tagName = ExtractTagNameFrom<T>();
            if (tagName == null) throw new ArgumentException("T");
            return CreateElement<T>(tagName.ToLowerInvariant());
        }

        static string ExtractTagNameFrom<T>()
        {
            string typeName = typeof (T).Name;
            if (typeName.StartsWith("I") && typeName.EndsWith("Element")) return typeName.Substring(1, typeName.Length - 8);
            return null;
        }

        public static T CreateElement<T>(string elementName)
            where T:class,IElement
        {
            return (T)CreateElementCore<T>(elementName);
        }

        static IElement CreateElementCore<T>(string elementName)
            where T : class, IElement
        {
            IEnumerable<Type> contentModels = GetContentModelFor<T>();
            var elementAttribs = GetAllowedAttributesFor<T>();
            var genericElemnt = new GenericElement(elementName)
            {
                Attributes = {AllowedAttributes = elementAttribs}
            };
            genericElemnt.ContentModel.AddRange(contentModels);
            return genericElemnt;
        }

        public static IDictionary<string,Func<IAttribute>> GetAllowedAttributesFor<T>()
        {
            var allAttributes = from prop in GetProperties(typeof (T)).Distinct()
                                where prop.CanRead
                                let attribs = Attribute.GetCustomAttributes(prop)
                                where attribs != null && attribs.Length > 0
                                let attrib = attribs.Where(a => typeof (XhtmlAttributeCore).IsAssignableFrom(a.GetType())).FirstOrDefault() as XhtmlAttributeCore
                                where attrib != null
                                select new {prop, attrib};
            return allAttributes.Distinct().ToDictionary(key => key.prop.Name, val => val.attrib.Factory(val.prop), StringComparer.OrdinalIgnoreCase);
        }
        static IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            foreach(var prop in type.GetProperties()) yield return prop;
            foreach(var parentType in type.GetInterfaces())
            {
                foreach(var pi in GetProperties(parentType)) yield return pi;
            }
        }

        public static IEnumerable<Type> GetContentModelFor<T>()
        {
            foreach(var interfaceType in typeof(T).GetInterfaces().Where(i=>i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IContentModel<,>)))
            {
                yield return interfaceType.GetGenericArguments()[1];
            }
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
