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
using OpenRasta.Configuration;
using OpenRasta.DI;
using OpenRasta.Pipeline.Contributors;
using OpenRasta.Testing;
using OpenRasta.Tests;
using OpenRasta.Pipeline;
using OpenRasta.Web.UriDecorators;

namespace UriDecoratorsController_Specification
{
    public class when_using_simple_decorators : openrasta_context
    {
        public void GivenUriDecorator<T>() where T:class,IUriDecorator { Resolver.AddDependency<IUriDecorator,T>(); }

        public void RegisterRepository()
        {
        }

        [Test]
        public void processing_a_decorator_changes_the_request_url()
        {
            RegisterRepository();
            Context.Request.Uri = new Uri("http://localhost/segment/hello");

            GivenUriDecorator<RemoveLastHello>();

            given_pipeline_contributor<UriDecoratorsContributor>();

            when_sending_notification<KnownStages.IUriMatching>();

            RemoveLastHello.ApplyWasCalled.ShouldBeTrue();
            Context.Request.Uri.ShouldBe(new Uri("http://localhost/segment"));
        }

    }

    public class RemoveLastHello : RemovingWordAtEndOfSegmentDecorator
    {
        public static bool ApplyWasCalled;
        public RemoveLastHello() : base("hello") { }

        public override void Apply()
        {
            ApplyWasCalled = true;
            base.Apply();
        }
    }

    public class RemovingWordAtEndOfSegmentDecorator : IUriDecorator
    {
        public RemovingWordAtEndOfSegmentDecorator(string wordToRemove) { WordToRemove = wordToRemove; }
        public string WordToRemove { get; set; }

        public bool Parse(Uri uri, out Uri processedUri)
        {
            if (uri.Segments[uri.Segments.Length - 1] == WordToRemove)
            {
                string path = string.Join("/", uri.Segments, 0, uri.Segments.Length - 1).Substring(1);
                processedUri = new UriBuilder(uri) {Path = path.Substring(0, path.Length - 1)}.Uri;
                return true;
            }
            processedUri = uri;
            return false;
        }

        public virtual void Apply() { }
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