namespace OpenRasta.Web.Markup.Attributes.Annotations
{
    public class NumberAttribute : PrimaryTypeAttributeCore
    {
        public NumberAttribute() :base(Factory<int?>){}
        public NumberAttribute(string attribName) : base(attribName,Factory<int?>) { }
    }
}