using System;

namespace OpenRasta.Web.Markup.Attributes
{
    public class EnumAttributeNode<T> : XhtmlAttributeNode<T>
    {
        public EnumAttributeNode(string name)
            : this(name, false) { }

        public EnumAttributeNode(string name, T defaultValue)
            : this(name, defaultValue, false)
        {
        }

        public EnumAttributeNode(string name, T defaultValue, bool renderWhenDefault)
            : this(name, renderWhenDefault)
        {
            DefaultValue = Write(defaultValue);
        }

        public EnumAttributeNode(string name, bool renderWhenDefault)
            : base(name, renderWhenDefault, Write, Read)
        {
        }

        static string Write(T value)
        {
            return Enum.GetName(typeof (T), value).ToLowerInvariant();
        }

        static T Read(string value)
        {
            return (T)Enum.Parse(typeof (T), value, true);
        }
    }
}