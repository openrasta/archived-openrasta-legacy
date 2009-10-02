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
using OpenRasta.Reflection;
using OpenRasta.Web.Markup;

//// ReSharper disable UnusedMember.Global
//// ReSharper disable UnusedParameter.Global

namespace OpenRasta.Codecs.SharpView
{
    public static class SourcedElementExtensions
    {
        public static T AddElements<T>(this T element, IEnumerable nodes) where T : class, IElement
        {
            if (element == null) return default(T);
            if (nodes == null) return element;

            foreach (INode value in nodes)
            {
                element.ChildNodes.Add(value);
            }
            return element;
        }

        public static T Current<T>(this IEnumerable<T> values)
        {
            throw new NotSupportedException("This is a marker method not to be called manually.");
        }

        public static T ForEach<T, TItem>(this T element, IEnumerable<TItem> values)
            where T : IElement
        {
            throw new NotSupportedException("This is a marker method to be used only with ");
        }

        public static T If<T>(this T element, object condition) where T : IElement
        {
            throw new NotSupportedException("This is a marker method not to be called manually.");
        }
        public static IEnumerable<TResult> SelectHtml<TSource, TResult>(this IEnumerable<TSource> source,
                                                                        object rootInstance,
                                                                        PropertyPath path,
                                                                        Func<TSource, TResult> selector)
        {
            if (source == null)
                yield break;
            int index = 0;
            foreach (var item in source)
            {
                try
                {
                    PropertyPath pathToAdd;
                    if (rootInstance != null)
                    {
                        PropertyPath rootPath = ObjectPaths.Get(rootInstance);
                        pathToAdd = new PropertyPath
                            {
                                TypePrefix = rootPath.TypePrefix,
                                TypeSuffix = rootPath.TypeSuffix + "." + path.TypeSuffix + ":" + index
                            };
                    }
                    else
                    {
                        pathToAdd = new PropertyPath
                            {
                                TypePrefix = path.TypePrefix,
                                TypeSuffix = path.TypeSuffix + ":" + index
                            };
                    }
                    ObjectPaths.Add(item, pathToAdd);
                    yield return selector(item);
                }
                finally
                {
                    ObjectPaths.Remove(item);
                    index++;
                }
            }
        }
    }
}

// ReSharper restore UnusedMember.Global
// ReSharper restore UnusedParameter.Global
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