namespace OpenRasta.CodeDom.Compiler
{
    public interface ICodeSnippetTextModifier : ICodeSnippetModifier
    {
        bool CanProcessString(string value);
        string ProcessString(string originalValue);
    }
}