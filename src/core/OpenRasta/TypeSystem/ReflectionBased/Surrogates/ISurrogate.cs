namespace OpenRasta.TypeSystem.ReflectionBased.Surrogates
{
    public interface ISurrogate
    {
        object Value { get; set; }
    }

    public interface ISurrogate<T> : ISurrogate
    {
        
    }
}