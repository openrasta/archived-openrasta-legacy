namespace OpenRasta.Web.Markup.Attributes
{
    public class NMTOKENSAttributeNode : CharacterSeparatedAttributeNode<string>
    {
        public NMTOKENSAttributeNode(string name) : base(name, " ", i=>i,i=>i)
        {
            Value = new CharacterSplitterCollection(" ");
        }
    }
}