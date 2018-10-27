using Microsoft.AspNetCore.Identity;

namespace Celia.io.Core.Auths.Abstractions
{
    public interface IApplicationRoleStore : IRoleStore<ApplicationRole>,
        IQueryableRoleStore<ApplicationRole>, IRoleClaimStore<ApplicationRole>
    {
    }
}