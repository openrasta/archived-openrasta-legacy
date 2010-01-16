using OpenRasta.Security;
using OpenRasta.Web;

namespace OpenBastard.Handlers
{
    public class UserHandler
    {
        [RequiresAuthentication]
        public OperationResult Delete(int id)
        {
            return new OperationResult.OK();
        }
    }
}