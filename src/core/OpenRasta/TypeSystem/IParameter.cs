namespace OpenRasta.TypeSystem
{
    public interface IParameter : IMember
    {
        IMethod Owner { get; }
        bool IsOptional { get; }
        object DefaultValue { get; }
        bool IsOutput { get; }
    }
}