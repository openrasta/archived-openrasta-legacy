namespace OpenRasta.Diagnostics
{
    public class NullErrorCollector : IErrorCollector
    {
        static readonly IErrorCollector INSTANCE = new NullErrorCollector();

        NullErrorCollector()
        {
        }

        public static IErrorCollector Instance
        {
            get { return INSTANCE; }
        }

        public void AddServerError(Error error)
        {
        }
    }
}