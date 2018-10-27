using Microsoft.AspNetCore.Identity;

namespace Celia.io.Core.Auths.Abstractions
{
    public interface IApplicationUserStore : IUserStore<ApplicationUser>,
        IQueryableUserStore<ApplicationUser>, IUserEmailStore<ApplicationUser>, IUserLoginStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>,
        IUserPhoneNumberStore<ApplicationUser>, IUserTwoFactorStore<ApplicationUser>, IUserSecurityStampStore<ApplicationUser>,
        IUserClaimStore<ApplicationUser>, IUserLockoutStore<ApplicationUser>, IUserRoleStore<ApplicationUser>
    {
        
    }
}