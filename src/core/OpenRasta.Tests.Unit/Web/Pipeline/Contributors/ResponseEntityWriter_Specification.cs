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
using Moq;
using NUnit.Framework;
using OpenRasta.Codecs;
using OpenRasta.IO;
using OpenRasta.Pipeline.Contributors;
using OpenRasta.Testing;
using OpenRasta.Tests;
using OpenRasta.Tests.Unit.Fakes;
using OpenRasta.TypeSystem;
using OpenRasta.Web;
using OpenRasta.Pipeline;

namespace ResponseEntityWriter_Specification
{
    public class when_there_is_no_response_entity : openrasta_context
    {
        [Test]
        public void no_entity_body_should_be_written()
        {
            given_pipeline_contributor<ResponseEntityWriterContributor>();
            given_response_entity(null);

            when_sending_notification<KnownStages.ICodecResponseSelection>()
                .ShouldBe(PipelineContinuation.Finished);

            Context.Response.Entity.Stream.Length.ShouldBe(0);
        }

        [Test]
        public void the_entity_headers_are_written()
        {
            given_pipeline_contributor<ResponseEntityWriterContributor>();
            given_response_entity(null);

            when_sending_notification<KnownStages.ICodecResponseSelection>()
                .ShouldBe(PipelineContinuation.Finished);

            Context.Response.HeadersSent.ShouldBeTrue();
        }
    }

    public class when_there_is_a_codec : openrasta_context
    {
        [Test]
        public void the_codec_configuration_is_assigned()
        {
            given_pipeline_contributor<ResponseEntityWriterContributor>();

            given_response_entity(new Fake());
            GivenAResponseCodec<CustomerCodec>(new object());

            when_sending_notification<KnownStages.ICodecResponseSelection>()
                .ShouldBe(PipelineContinuation.Finished);

            Context.Response.Entity.Headers["ENTITY_TYPE"].ShouldBe("Fake");
            Context.Response.Entity.Codec.Configuration.ShouldNotBeNull();
        }

        [Test]
        public void the_correct_content_is_returned()
        {
            given_pipeline_contributor<ResponseEntityWriterContributor>();
            given_response_entity(new Fake());
            GivenAResponseCodec<CustomerCodec>();

            when_sending_notification<KnownStages.ICodecResponseSelection>()
                .ShouldBe(PipelineContinuation.Finished);

            Context.Response.Entity.Headers["ENTITY_TYPE"].ShouldBe("Fake");
        }

        void GivenAResponseCodec<TCodec>()
        {
            GivenAResponseCodec<TCodec>(null);
        }

        void GivenAResponseCodec<TCodec>(object config)
        {
            if (Context.PipelineData.ResponseCodec != null)
                Context.PipelineData.ResponseCodec = null;

            Context.PipelineData.ResponseCodec = CodecRegistration.FromResourceType(typeof(object),
                                                                       typeof(TCodec),
                                                                       TypeSystems.Default,
                                                                       new MediaType("application/unknown"),
                                                                       null,
                                                                       config, false);
        }
    }

    public class when_writing_the_entity : openrasta_context
    {
        [Test]
        public void the_content_length_is_defined_properly()
        {
            given_pipeline_contributor<ResponseEntityWriterContributor>();
            given_response_entity(new Fake());

            GivenAContentTypeWriter((instance, entity, codecParams) => entity.Stream.Write(new byte[50]));

            when_sending_notification<KnownStages.ICodecResponseSelection>()
                .ShouldBe(PipelineContinuation.Finished);

            Context.Response.Headers.ContentLength
                .ShouldBe(50);
        }

        IMediaTypeWriter GivenAContentTypeWriter(Action<object, IHttpEntity, string[]> code)
        {
            var mock = new Mock<IMediaTypeWriter>();
            mock.Expect(writer => writer.WriteTo(It.IsAny<object>(), It.IsAny<IHttpEntity>(), It.IsAny<string[]>()))
                .Callback(code);
            return (Context.Response.Entity.Codec = mock.Object) as IMediaTypeWriter;
        }

        void GivenAResponseCodec(IMediaTypeWriter mock)
        {
            Context.Response.Entity.Codec = mock;
        }
    }

    public class Fake
    {
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