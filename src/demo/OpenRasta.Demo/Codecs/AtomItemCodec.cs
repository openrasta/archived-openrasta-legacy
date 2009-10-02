using System.ServiceModel.Syndication;
using System.Xml;
using OpenRasta.Codecs;

namespace OpenRasta.Demo.Codecs
{
    [MediaType("application/atom+xml;q=0.9", "atom")]
    [MediaType("application/xml;q=0.9", "atom")]
    public class AtomItemCodec : SyndicationCodecBase<SyndicationItem>
    {
        protected override void WriteTo(SyndicationItem item, XmlWriter writer)
        {
            item.SaveAsAtom10(writer);
        }
    }
}