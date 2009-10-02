using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using OpenRasta.Demo.Resources;
using OpenRasta.Demo.SyndicationExtensions;

namespace OpenRasta.Demo.Handlers
{
    /// <summary>
    /// Retrieves syndicated articles as feed or item
    /// </summary>
    public class ArticleHandler
    {
        public SyndicationFeed Get()
        {
            return new SyndicationFeed(new List<SyndicationItem>
                                           {
                                               SyndicationItemFactory.CreateExampleItemWithId(1),
                                               SyndicationItemFactory.CreateExampleItemWithId(2),
                                               SyndicationItemFactory.CreateExampleItemWithId(3),
                                           });
        }

        public SyndicationItem Get(int id)
        {
            SyndicationItem item = SyndicationItemFactory.CreateExampleItemWithId(id);
            item.Links.Add(SyndicationItemFactory.GetLink(id));
            return item;
        }
    }

    public static class SyndicationItemFactory
    {
        /// <summary>
        /// Creates the fake item with the given id and a couple of example links and author.  
        /// Every item with an even ID gets a feedsync extension
        /// </summary>
        /// <param name="id">The id.</param>
        public static SyndicationItem CreateExampleItemWithId(int id)
        {
            var item = new SynchronizableArticle(String.Format("A title with ID {0}", id),
                                                 String.Empty, null,
                                                 id.ToString(),
                                                 DateTime.Now.Subtract(TimeSpan.FromDays(Random.Next(30))));
            if (id % 2 == 0)
                item.FeedSync = new FeedSync
                                    {
                                        Deleted = false,
                                        Id = id.ToString(),
                                        NoConflicts = true,
                                        Updates = 6
                                    };

            item.Authors.Add(GetRandomAuthor());
            if(id % 3 == 0)
                item.Authors.Add(GetRandomAuthor());

            item.Links.Add(GetLink(1));
            item.Links.Add(GetLink(2));

            item.Content = SyndicationContent.CreateHtmlContent(
                "<html><head></head><body><p>Some test text & escaping test</p><body></html>");

            return item;
        }

        private static SyndicationPerson[] authors = new SyndicationPerson[]
                                                         {
                                                            new SyndicationPerson(
                                                                "sauron@cheesygothicmountain.com", 
                                                                "Sauron", 
                                                                "http://haveyouseenmyring.com"),
                                                            new SyndicationPerson(
                                                                "gandalf@discountbeardcare.co.uk", 
                                                                "Gandalf the Greeeeeeyt", 
                                                                "http://trollsnotwelcome.com"),
                                                            new SyndicationPerson(
                                                                "frodo@theshire.org.ie", 
                                                                "Frodo", 
                                                                "http://cheapclimbinglessons.com")
                                                         };

        private static readonly Random Random = new Random(Environment.TickCount);
        private static SyndicationPerson GetRandomAuthor()
        {
            return authors[Random.Next(authors.GetLowerBound(0), authors.GetUpperBound(0) + 1)];
        }

        public static SyndicationLink GetLink(int id)
        {
            return new SyndicationLink(new Uri("http://www.google.com/" + id))
                       {
                           MediaType = "text/xhtml",
                           RelationshipType = "somerelationship",
                           Title = "Foobar"
                       };
        }
    }
}