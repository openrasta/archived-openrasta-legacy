using OpenRasta.OperationModel;
using OpenRasta.OperationModel.Interceptors;
using OpenRasta.Web;

namespace OpenRasta.Security
{
    public class RequiresRoleInterceptor : OperationInterceptor
    {
        ICommunicationContext _context;

        public RequiresRoleInterceptor(ICommunicationContext context)
        {
            _context = context;
        }

        public string Role { get; set; }
        public override bool BeforeExecute(IOperation operation)
        {
            var isAuthorized = Role == null || _context.User.IsInRole(Role);
            if (!isAuthorized)
            {
                _context.OperationResult = new OperationResult.Unauthorized();
            }
            return isAuthorized;
        }
    }
}