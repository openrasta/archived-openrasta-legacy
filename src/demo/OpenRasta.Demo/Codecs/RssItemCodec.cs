using System.ServiceModel.Syndication;
using System.Xml;
using OpenRasta.Codecs;

namespace OpenRasta.Demo.Codecs
{
    [MediaType("application/rss+xml;q=0.8", "rss")]
    [MediaType("application/xml;q=0.7", "rss")]
    public class RssItemCodec : SyndicationCodecBase<SyndicationItem>
    {
        protected override void WriteTo(SyndicationItem item, XmlWriter writer)
        {
            item.SaveAsRss20(writer);
        }
    }
}