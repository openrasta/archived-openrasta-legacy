namespace OpenRasta.TypeSystem
{
    public interface IParameterBuilder : ITypeBuilder
    {
        IParameter Parameter { get; set; }
    }
}