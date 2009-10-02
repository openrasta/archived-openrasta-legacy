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

namespace OpenRasta.Data
{
    public static class QueryableExtensions
    {
        public static PagedData<T> SelectPagedData<T>(this IQueryable<T> source, int requestedPage, int pageSize) { return SelectPagedData(source, requestedPage, pageSize, null); }

        public static PagedData<T> SelectPagedData<T>(this IQueryable<T> source, int requestedPage, int pageSize,
                                                      Func<PagedData<T>, Uri> pageUriCreator)
        {
            if (requestedPage < 1)
                throw new ArgumentOutOfRangeException("requestedPage", "The requested page cannot be less than 1");
            if (pageSize < 1) throw new ArgumentOutOfRangeException("pageSize", "The page size cannot be less than 1");

            int totalItemCount = source.Count();
            int totalPageCount = totalItemCount/pageSize + (totalItemCount%pageSize > 0 ? 1 : 0);
            
            // assign null value
            pageUriCreator = pageUriCreator ?? (((t) => null));

            if (requestedPage != 1 && requestedPage > totalPageCount)
                throw new ArgumentOutOfRangeException("requestedPage",
                                                      string.Format("There is no page {0}", requestedPage));

            var currentPage = new PagedData<T> {
                                                   CurrentPage = requestedPage,
                                                   PageSize = pageSize
                                               };
            currentPage.PageUri = pageUriCreator(currentPage);
            List<PagedData<T>> availablePages = new List<PagedData<T>>();
            for (int i = 1; i <= totalPageCount; i++)
            {
                if (i == requestedPage)
                    continue;
                var newPage = new PagedData<T> {CurrentPage = i, PageSize = pageSize};
                newPage.PageUri = pageUriCreator(newPage);
                availablePages.Add(newPage);
            }
            currentPage.OtherPages = availablePages;

            var start = (requestedPage == 1) ? source : source.Skip((requestedPage - 1)*pageSize);
            currentPage.Items = start.Take(pageSize).ToList();
            currentPage.ResultCount = totalItemCount;

            return currentPage;
        }
    }

    public class PagedData<T>
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int ResultCount { get; set; }
        public Uri PageUri { get; set; }

        public IList<T> Items { get; set; }
        public IList<PagedData<T>> OtherPages { get; set; }
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