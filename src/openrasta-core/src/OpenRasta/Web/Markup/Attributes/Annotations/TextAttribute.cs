namespace OpenRasta.Web.Markup.Attributes.Annotations
{
    public class TextAttribute : CDATAAttribute
    {
        public TextAttribute(){}
        public TextAttribute(string name) : base(name) { }
        
    }
    public class LengthAttribute : CDATAAttribute
    {
        public LengthAttribute() { }
        public LengthAttribute(string name) : base(name) { }

    }
}