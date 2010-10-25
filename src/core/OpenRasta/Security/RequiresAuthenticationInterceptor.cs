using OpenRasta.OperationModel;
using OpenRasta.OperationModel.Interceptors;
using OpenRasta.Web;

namespace OpenRasta.Security
{
    public class RequiresAuthenticationInterceptor : OperationInterceptor
    {
        readonly ICommunicationContext _context;

        public RequiresAuthenticationInterceptor(ICommunicationContext context)
        {
            _context = context;
        }

        public override bool BeforeExecute(IOperation operation)
        {
            if (_context.User == null || _context.User.Identity == null || !_context.User.Identity.IsAuthenticated)
            {
                _context.OperationResult = new OperationResult.Unauthorized();
                return false;
            }

            return true;
        }
    }
}