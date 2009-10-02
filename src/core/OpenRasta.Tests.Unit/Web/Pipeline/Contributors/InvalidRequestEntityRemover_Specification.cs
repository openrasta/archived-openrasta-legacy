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
using NUnit.Framework;
using OpenRasta.Testing;
using OpenRasta.Tests;
using OpenRasta.Web;

namespace InvalidRequestEntityRemover_Specification
{
    //[TestFixture]
    //public class when_the_verb_doesnt_allow_a_request_entity : openrasta_context
    //{
    //    [Test]
    //    public void entity_body_is_removed_and_the_content_is_ignored()
    //    {
    //        Context.Request.HttpMethod = "HEAD";
    //        Context.Request.ContentLength = 10;
    //        Context.Request.Entity.ContentType = new MediaType("text/plain");

    //        given_pipeline_contributor<InvalidRequestEntityRemover>();
    //        when_sending_notification<RequestEntityCodecResolver>();

    //        Context.Request.ContentLength.ShouldBe(0);
    //        Context.Request.Entity.ContentType.ShouldBeNull();
    //    }
    //}
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