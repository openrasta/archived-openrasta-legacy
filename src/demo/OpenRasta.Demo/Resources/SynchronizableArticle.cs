using System;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Xml.Serialization;
using OpenRasta.Demo.SyndicationExtensions;

namespace OpenRasta.Demo.Resources
{
    /// <summary>
    /// Demo of a syndication item which has a feedsync.org (top level only, no sequence/when/by) 
    /// extension
    /// </summary>
    public class SynchronizableArticle : SyndicationItem
    {
        #region Constructors

        public SynchronizableArticle()
        {
        }

        public SynchronizableArticle(int id) : base(string.Empty, string.Empty, null, id.ToString(), DateTime.MinValue)
        {
        }

        public SynchronizableArticle(string title, string content, Uri itemAlternateLink, string id, DateTimeOffset lastUpdatedTime)
            : base(title, content, itemAlternateLink, id, lastUpdatedTime)
        {
        }
        
        #endregion

        public FeedSync FeedSync
        {
            get
            {
                SyndicationElementExtension extension = ElementExtensions.FirstExtensionOfType<FeedSync>();
                if(extension == null)
                    return null;

                return extension.GetObject<FeedSync>();
            }
            set
            {
                SyndicationElementExtension extension = ElementExtensions.FirstExtensionOfType<FeedSync>();
                if(extension != null)
                {
                    this.ElementExtensions.Remove(extension);
                }
                this.ElementExtensions.Add(value, new XmlSerializer(typeof(FeedSync)));
            }
        }
    }
}