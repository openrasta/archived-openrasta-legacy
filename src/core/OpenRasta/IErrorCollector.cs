namespace OpenRasta.Diagnostics
{
    public interface IErrorCollector
    {
        void AddServerError(Error error);
    }
}