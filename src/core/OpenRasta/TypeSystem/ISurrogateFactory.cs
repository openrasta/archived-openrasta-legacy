namespace OpenRasta.TypeSystem
{
    public interface ISurrogateFactory
    {
        IProperty FindSurrogate(IProperty property);
        IType FindSurrogate(IType type);
    }
}