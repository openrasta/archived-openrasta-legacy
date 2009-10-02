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
using NUnit.Framework;
using OpenRasta.Data;
using OpenRasta.Testing;

namespace PagedData_Specification
{
    [TestFixture]
    public class when_selecting_pages : context
    {
        readonly IQueryable<int> rangeOfValues = Enumerable.Range(1, 20).AsQueryable();

        [Test]
        public void asking_for_a_page_when_it_doesnt_exist_because_there_are_not_enough_results_will_throw_an_exception()
        {
            Executing(() => rangeOfValues.SelectPagedData(3, 10, null))
                .ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Test]
        public void asking_for_an_invalid_page_number_raises_an_exception()
        {
            var listToQuery = Enumerable.Range(1, 20).AsQueryable();

            Executing(() => rangeOfValues.SelectPagedData(0, 10, null))
                .ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Test]
        public void asking_for_an_invalid_page_size_raises_an_exception()
        {
            Executing(() => rangeOfValues.SelectPagedData(0, 0, null))
                .ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Test]
        public void requesting_page_1_is_a_valida_action_even_when_tehre_are_no_results()
        {
            var page = new List<int>().AsQueryable().SelectPagedData(1, 10);
            page.CurrentPage.ShouldBe(1);
        }

        [Test]
        public void requesting_the_second_page_for_a_page_size_of_5_returns_5_items_and_4_pages()
        {
            var page = rangeOfValues.SelectPagedData(2, 5, null);
            page.Items.Count.ShouldBe(5);
            page.CurrentPage.ShouldBe(2);
            page.OtherPages.Count.ShouldBe(3);
        }
        [Test]
        public void there_are_two_pages_when_the_page_count_is_19()
        {
            var page = rangeOfValues.SelectPagedData(1, 19);
            page.Items.Count.ShouldBe(19);
            page.CurrentPage.ShouldBe(1);
            page.OtherPages.Count.ShouldBe(1);
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