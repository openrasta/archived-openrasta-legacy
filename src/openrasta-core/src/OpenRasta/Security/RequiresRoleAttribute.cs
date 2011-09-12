using System;
using System.Collections.Generic;
using OpenRasta.DI;
using OpenRasta.OperationModel;
using OpenRasta.OperationModel.Interceptors;

namespace OpenRasta.Security
{
    public class RequiresRoleAttribute : OpenRasta.OperationModel.Interceptors.InterceptorProviderAttribute
    {
        readonly string _roleName;

        public RequiresRoleAttribute(string roleName)
        {
            if (roleName == null) throw new ArgumentNullException("roleName");
            _roleName = roleName;
        }

        public override IEnumerable<IOperationInterceptor> GetInterceptors(IOperation operation)
        {
            yield return DependencyManager.GetService<RequiresAuthenticationInterceptor>();
            var roleInterceptor = DependencyManager.GetService<RequiresRoleInterceptor>();
            roleInterceptor.Role = _roleName;
            yield return roleInterceptor;
        }
    }
}