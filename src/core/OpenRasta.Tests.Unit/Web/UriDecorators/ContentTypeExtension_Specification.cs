using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApplicationXWwwUrlformEncodedCodec_Specification;
using NUnit.Framework;
using OpenRasta.Codecs;
using OpenRasta.Testing;
using OpenRasta.Web;
using OpenRasta.TypeSystem;
using OpenRasta.Web.UriDecorators;

namespace OpenRasta.Tests.Unit.Web.UriDecorators
{
    public class when_rewriting_a_uri_with_extension : context.content_type_extension
    {
        [Test]
        public void the_rewritten_uri_is_relative_to_the_app_base()
        {
            given_resource<Customer>("/customer/{id}");
            given_request_uri("http://localhost/vdir/customer/1.xml");
            given_context_applicationBase("http://localhost/vdir");

            when_parsing();
            ParsingResult.ShouldBeTrue();
            ProcessedUri.ShouldBe("http://localhost/vdir/customer/1");

            when_applying();
            Context.PipelineData.ResponseCodec.CodecType.ShouldBe<XmlCodec>();
            Context.Response.Entity.ContentType.ShouldBe(MediaType.Xml);

        }

    }
    namespace context
    {
        public class content_type_extension : openrasta_context
        {
            protected Uri ProcessedUri;

            public content_type_extension()
            {
                
            }
            protected void given_resource<T>(string template)
            {
                var resourceKey = TypeSystem.FromClr<T>();
                UriResolver.Add(new UriRegistration(template, resourceKey));
                Codecs.Add(new CodecRegistration(typeof(XmlCodec),
                    resourceKey,
                    false,  
                    MediaType.Xml,
                    new[]{"xml"},null,false));
            }
            protected void when_parsing()
            {
                Parser = Parser ?? new ContentTypeExtensionUriDecorator(Context, UriResolver, Codecs, TypeSystem);
                ParsingResult = Parser.Parse(Request.Uri, out ProcessedUri);
            }
            protected void when_applying()
        {
                Parser.Apply();
        }

            protected bool ParsingResult { get; set; }

            protected ContentTypeExtensionUriDecorator Parser { get; set; }
        }
    }
}
