using System.Xml.Serialization;

namespace OpenRasta.Demo.SyndicationExtensions
{
    [XmlRoot(ElementName = "sync", Namespace = SyncNs, IsNullable = false)]
    public class FeedSync
    {
        public const string SyncNs = "http://feedsync.org/2007/feedsync";

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "updates")]
        public int Updates { get; set; }

        [XmlAttribute(AttributeName = "deleted")]
        public bool Deleted { get; set; }

        [XmlAttribute(AttributeName = "noconflicts")]
        public bool NoConflicts { get; set; }
    }
}