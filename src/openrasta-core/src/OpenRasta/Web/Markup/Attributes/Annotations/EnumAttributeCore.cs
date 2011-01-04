using System;

namespace OpenRasta.Web.Markup.Attributes
{
    public abstract class EnumAttributeCore : XhtmlAttributeCore
    {
        readonly Func<string, Func<IAttribute>> _factory;
        protected EnumAttributeCore() { }
        protected EnumAttributeCore(Func<string, Func<IAttribute>> factory) : this(null, factory) { }
        protected EnumAttributeCore(string attribName, Func<string, Func<IAttribute>> factory)
            : base(attribName)
        {
            _factory = factory;
        }

        public static Func<IAttribute> Factory<T>(string propertyName)
        {
            return () => (IAttribute)new EnumAttributeNode<T>(propertyName);
        }
        protected override Func<IAttribute> Factory(string propertyName)
        {
            return _factory(propertyName);
        }
    }
}