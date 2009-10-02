using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenRasta.Web.Markup.Elements;

namespace OpenRasta.Web.Markup
{
    public static class IMetaElementExtensions
    {
        public static T Scheme<T>(this T element, string scheme)
            where T:IMetaElement
        {
            element.Scheme = scheme;
            return element;
        }
        public static T Content<T>(this T element, string content)
           where T : IMetaElement
        {
            element.Content = content;
            return element;
        }
        public static T HttpEquiv<T>(this T element, string httpEquiv)
          where T : IMetaElement
        {
            element.HttpEquiv = httpEquiv;
            return element;
        }
        public static IMetaElement Name(this IMetaElement element, string name)
        {
            element.Name = name;
            return element;
        }
    }
}
