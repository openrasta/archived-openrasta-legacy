using OpenRasta.Web;

namespace OpenRasta.Diagnostics
{
    public class OperationContextErrorCollector : IErrorCollector
    {
        readonly ICommunicationContext _context;

        public OperationContextErrorCollector(ICommunicationContext context)
        {
            _context = context;
        }

        public void AddServerError(Error error)
        {
            _context.ServerErrors.Add(error);
        }
    }
}