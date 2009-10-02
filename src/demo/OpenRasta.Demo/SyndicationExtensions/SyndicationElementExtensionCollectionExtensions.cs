using System.Linq;
using System.ServiceModel.Syndication;

namespace OpenRasta.Demo.SyndicationExtensions
{
    public static class SyndicationElementExtensionCollectionExtensions
    {
        public static SyndicationElementExtension FirstExtensionOfType<T>(this SyndicationElementExtensionCollection elementExtensions)
        {            
            return elementExtensions.FirstOrDefault(x => x.GetObject<T>().GetType() == typeof(T));
        }
    }
}