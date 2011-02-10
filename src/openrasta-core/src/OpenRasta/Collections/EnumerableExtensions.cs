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
using System.Collections.ObjectModel;
using System.Linq;

namespace OpenRasta.Collections
{
    public static class EnumerableExtensions
    {
        public static bool Contains(this IEnumerable<string> source, string value, StringComparison comparisonType)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            foreach (string element in source)
            {
                if (string.Compare(element, value, comparisonType) == 0)
                    return true;
            }
            return false;
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            if (list == null)
                return null;
            foreach (var value in list)
                action(value);
            return list;
        }

        public static TDestination[] ToArray<TSource, TDestination>(this IEnumerable<TSource> source, 
                                                                    Func<TSource, TDestination> converter)
        {
            return source.ToList(converter).ToArray();
        }

        public static Dictionary<string, TElement> ToCaseInsensitiveDictionary<TSource, TElement>(
            this IEnumerable<TSource> toConvert, Func<TSource, string> keyFinder, Func<TSource, TElement> elementFinder)
        {
            return toConvert.ToDictionary(keyFinder, elementFinder, StringComparer.CurrentCultureIgnoreCase);
        }

        public static Collection<T> ToCollection<T>(this IEnumerable<T> source)
        {
            return new Collection<T>(source.ToList());
        }
        public static TCollectionType ToCollection<TSource, TDestination, TCollectionType>(
            this IEnumerable<TSource> source, Func<TSource, TDestination> converter)
            where TCollectionType : ICollection<TDestination>, new()
        {
            var t = new TCollectionType();
            foreach (var elementToConvert in source)
            {
                try
                {
                    t.Add(converter(elementToConvert));
                }
                catch
                {
                }
            }
            return t;
        }

        public static IList<TDestination> ToList<TSource, TDestination>(this IEnumerable<TSource> source, 
                                                                        Func<TSource, TDestination> converter)
        {
            return source.Select(converter).ToList();
        }

        public static bool TryForEach<T>(this IEnumerable<T> collection, Action<T> forAction)
        {
            bool hit = false;
            foreach (var item in collection)
            {
                hit = true;
                forAction(item);
            }
            return hit;
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