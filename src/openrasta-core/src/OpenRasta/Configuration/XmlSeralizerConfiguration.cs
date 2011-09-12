using System.Xml.Serialization;
using OpenRasta.Codecs;
using OpenRasta.Configuration.Fluent;

namespace OpenRasta.Configuration
{
    public static class XmlSeralizerConfiguration
    {
        /// <summary>
        /// Enables reading and writing resource representations using the framework's <see cref="XmlSerializer"/>.
        /// </summary>
        /// <param name="parentDefinition"></param>
        /// <returns></returns>
        public static ICodecDefinition AsXmlSerializer(this ICodecParentDefinition parentDefinition)
        {
            return parentDefinition.TranscodedBy<XmlSerializerCodec>(null);
        }
    }
}