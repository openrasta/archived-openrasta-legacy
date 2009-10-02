using System;
using System.ServiceModel.Syndication;
using System.Xml;
using NUnit.Framework;
using OpenRasta.Codecs;
using OpenRasta.Configuration;
using OpenRasta.Configuration.Fluent;
using OpenRasta.Testing;
using OpenRasta.Web;

namespace OpenRasta.Tests.Integration.Regressions
{
    public class Handlers_not_selected_correctly : server_context
    {
        public Handlers_not_selected_correctly()
        {
            ConfigureServer(() =>
            {
                ResourceSpace.Has.ResourcesOfType<SyndicationFeed>()
                    .AtUri("/news/since/{year}/{month}/{day}")
                    .HandledBy<ArticlesSinceHandler>()
                    .TranscodedBy<AtomFeedCodec>(null);

                ResourceSpace.Has.ResourcesOfType<SyndicationItem>()
                    .AtUri("/news/{id}")
                    .HandledBy<ArticleHandler>()
                    .TranscodedBy<AtomItemCodec>(null);
            });
        }

        protected SyndicationFeed Feed { get; set; }

        protected SyndicationItem Item { get; set; }

        [Test]
        public void the_handler_for_individual_items_is_selected()
        {
            given_request("GET", "/news/23");

            when_reading_response_as_item();

            Item.Id.ShouldBe("23");
        }

        [Test]
        public void the_handler_with_the_most_attributes_is_selected()
        {
            given_request("GET", "/news/since/2001/02/03");
            when_reading_response_as_feed();
            
            Feed.LastUpdatedTime.Year.ShouldBe(2001);
            Feed.LastUpdatedTime.Month.ShouldBe(02);
            Feed.LastUpdatedTime.Day.ShouldBe(03);
        }

        void when_reading_response_as_feed()
        {
            when_reading_response();
            Feed = SyndicationFeed.Load(XmlReader.Create(TheResponse.GetResponseStream().ShouldNotBeNull()));
        }

        void when_reading_response_as_item()
        {
            when_reading_response();
            Item = SyndicationItem.Load(XmlReader.Create(TheResponse.GetResponseStream().ShouldNotBeNull()));
        }
    }

    public class ArticleHandler
    {
        public SyndicationItem Get(int id)
        {
            return new SyndicationItem { Id = id.ToString() };
        }
    }

    public class ArticlesSinceHandler
    {
        public SyndicationFeed Get(DateTime since)
        {
            return new SyndicationFeed("title", "description", "http://localhost/test.atom".ToUri(), "id", since);
        }
    }

    [MediaType("application/atom+xml;q=0.9", "atom")]
    [MediaType("application/atom;q=0.9", "atom")]
    [SupportedType(typeof(SyndicationFeed))]
    public class AtomFeedCodec : SyndicationCodecBase<SyndicationFeed>
    {
        protected override void WriteTo(SyndicationFeed item, XmlWriter writer)
        {
            item.SaveAsAtom10(writer);
        }
    }

    [MediaType("application/atom+xml;q=0.9", "atom")]
    [MediaType("application/atom;q=0.9", "atom")]
    [SupportedType(typeof(SyndicationItem))]
    public class AtomItemCodec : SyndicationCodecBase<SyndicationItem>
    {
        protected override void WriteTo(SyndicationItem item, XmlWriter writer)
        {
            item.SaveAsAtom10(writer);
        }
    }

    public abstract class SyndicationCodecBase<TEntity> : IMediaTypeWriter where TEntity : class
    {
        public object Configuration
        {
            get { return null; }
            set { }
        }

        public void WriteTo(object entity, IHttpEntity response, string[] codecParameters)
        {
            var item = entity as TEntity;
            if (item == null)
                throw new ArgumentException("Entity was not a " + typeof(TEntity).Name, "entity");

            using (var writer = XmlWriter.Create(response.Stream))
            {
                WriteTo(item, writer);
            }
        }

        protected abstract void WriteTo(TEntity item, XmlWriter writer);
    }
}