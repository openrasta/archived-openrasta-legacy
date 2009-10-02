namespace OpenRasta.Web.Markup.Attributes.Annotations
{
    public class CharacterAttribute : PrimaryTypeAttributeCore
    {
        public CharacterAttribute() : base(Factory<char?>) { }
        public CharacterAttribute(string attribName) : base(attribName, Factory<char?>) { }
    }
}