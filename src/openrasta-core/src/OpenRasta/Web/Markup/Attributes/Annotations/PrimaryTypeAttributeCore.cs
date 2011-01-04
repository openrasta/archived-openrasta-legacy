using System;

namespace OpenRasta.Web.Markup.Attributes.Annotations
{
    public abstract class PrimaryTypeAttributeCore : XhtmlAttributeCore
    {
        readonly Func<string, Func<IAttribute>> _factory;
        protected PrimaryTypeAttributeCore() {}
        protected PrimaryTypeAttributeCore(Func<string,Func<IAttribute>> factory) : this(null,factory) {}
        protected PrimaryTypeAttributeCore(string attribName,Func<string,Func<IAttribute>> factory) : base(attribName)
        {
            _factory = factory;
        }

        public static Func<IAttribute> Factory<T>(string propertyName)
        {
            return () => (IAttribute) new PrimaryTypeAttributeNode<T>(propertyName);
        }
        protected override Func<IAttribute> Factory(string propertyName)
        {
            return _factory(propertyName);
        }
    }
}