using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Celia.io.Core.Auths.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Security.Claims;
using Celia.io.Core.Utils;

namespace Celia.io.Core.Auths.DataAccess.EfCore
{
    public class EfCoreUserStore : IApplicationUserStore
    {
        private readonly ApplicationDbContext _context = null;
        private readonly ILogger logger = null;

        public EfCoreUserStore(ILogger<EfCoreUserStore> logger, ApplicationDbContext context)
        {
            this._context = context ?? throw new ArgumentNullException(
                nameof(context));
            this.logger = logger ?? throw new ArgumentOutOfRangeException(nameof(logger));
        }

        public Task<string> GetUserIdAsync(ApplicationUser user,
            CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user),
                    "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(user.Id))
                user.Id = IdGenerator.GenerateObjectId().ToString();

            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(ApplicationUser user,
            CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user),
                    "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.UserName);
        }

        public Task SetUserNameAsync(ApplicationUser user, string userName,
            CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user),
                    "Parameter user is not set to an instance of an object.");
            }

            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException(nameof(userName),
                    "Parameter userName cannot be null or empty.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            user.UserName = userName;
            return Task.FromResult<object>(null);
        }

        public Task<string> GetNormalizedUserNameAsync(ApplicationUser user,
            CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user),
                    "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task SetNormalizedUserNameAsync(ApplicationUser user,
            string normalizedName, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            if (string.IsNullOrEmpty(normalizedName))
            {
                throw new ArgumentNullException(nameof(normalizedName), "Parameter normalizedName cannot be null or empty.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            user.NormalizedUserName = normalizedName;
            return Task.FromResult<object>(null);
        }

        public Task<IdentityResult> CreateAsync(
            ApplicationUser user, CancellationToken cancellationToken)
        {
            this._context.Users.Add(user);

            return this._context.SaveChangesAsync(cancellationToken)
                .ContinueWith((u) =>
                {
                    if (u.IsCompleted)
                        return IdentityResult.Success;
                    else if (u.IsCanceled)
                        return IdentityResult.Failed(
                            new IdentityError()
                            {
                                Code = "400",
                                Description = "User canceled"
                            });

                    return IdentityResult.Failed();
                });
        }

        public Task<IdentityResult> UpdateAsync(ApplicationUser user,
            CancellationToken cancellationToken)
        {
            return this._context.Users.FindAsync(
                new object[] { user.Id }, cancellationToken)
                .ContinueWith<IdentityResult>((u) =>
                {
                    if (u.IsCanceled)
                        return IdentityResult.Failed(
                            new IdentityError()
                            {
                                Code = "400",
                                Description = "User canceled"
                            });

                    if (u.IsCompleted)
                    {
                        u.Wait();
                        u.Result.Email = user.Email;
                        u.Result.EmailConfirmed = user.EmailConfirmed;
                        u.Result.NormalizedEmail = user.NormalizedEmail;
                        u.Result.NormalizedUserName = user.NormalizedUserName;
                        u.Result.PhoneNumber = user.PhoneNumber;
                        u.Result.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
                        u.Result.UserName = user.UserName;

                        Task<IdentityResult> t = _context.SaveChangesAsync(cancellationToken)
                        .ContinueWith((r) =>
                        {
                            if (r.IsCompleted)
                                return IdentityResult.Success;
                            else if (r.IsCanceled)
                                return IdentityResult.Failed(
                                    new IdentityError()
                                    {
                                        Code = "400",
                                        Description = "User canceled"
                                    });

                            return IdentityResult.Failed();
                        });

                        t.Wait();
                        return t.Result;
                    }

                    return IdentityResult.Failed();
                });
        }

        public Task<IdentityResult> DeleteAsync(
            ApplicationUser user, CancellationToken cancellationToken)
        {
            return this._context.Users.FindAsync(
                new object[] { user.Id }, cancellationToken)
                   .ContinueWith<IdentityResult>((u) =>
                   {
                       u.Wait();

                       if (u.IsCompleted)
                       {
                           if (u.Result != null)
                               _context.Users.Remove(u.Result);
                           var t2 = _context.SaveChangesAsync(cancellationToken)
                             .ContinueWith((r2) =>
                             {
                                 if (r2.IsCompleted)
                                     return IdentityResult.Success;

                                 else if (r2.IsCanceled)
                                     return IdentityResult.Failed(
                                  new IdentityError()
                                  {
                                      Code = "400",
                                      Description = "User canceled"
                                  });

                                 return IdentityResult.Failed();
                             });

                           t2.Wait();
                           return t2.Result;
                       }

                       else if (u.IsCanceled)
                           return IdentityResult.Failed(
                        new IdentityError()
                        {
                            Code = "400",
                            Description = "User canceled"
                        });

                       return IdentityResult.Failed();
                   });
        }

        public Task<ApplicationUser> FindByIdAsync(string userId,
            CancellationToken cancellationToken)
        {
            return _context.Users.FindAsync(
                new object[] { userId }, cancellationToken);
        }

        public Task<ApplicationUser> FindByNameAsync(
            string normalizedUserName, CancellationToken cancellationToken)
        {
            return _context.Users.FirstOrDefaultAsync(
                m => !string.IsNullOrEmpty(m.NormalizedUserName)
                && m.NormalizedUserName.Equals(normalizedUserName),
                cancellationToken);
        }

        public Task SetEmailAsync(ApplicationUser user,
            string email, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email), "Parameter email cannot be null or empty.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            user.Email = email;
            return Task.FromResult<object>(null);
        }

        public Task<string> GetEmailAsync(ApplicationUser user,
            CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user),
                    "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(ApplicationUser user,
            CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(ApplicationUser user,
            bool confirmed, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            user.EmailConfirmed = confirmed;
            return Task.FromResult<object>(null);
        }

        public Task<ApplicationUser> FindByEmailAsync(
            string normalizedEmail, CancellationToken cancellationToken)
        {
            var temp = normalizedEmail ?? throw new ArgumentNullException(nameof(normalizedEmail));

            return _context.Users.FirstOrDefaultAsync(m => m.NormalizedEmail.Equals(
                normalizedEmail, StringComparison.InvariantCultureIgnoreCase), cancellationToken);
        }

        public Task<string> GetNormalizedEmailAsync(
            ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(
            ApplicationUser user, string normalizedEmail,
            CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            if (string.IsNullOrEmpty(normalizedEmail))
            {
                throw new ArgumentNullException(nameof(normalizedEmail), "Parameter normalizedEmail cannot be null or empty.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            user.NormalizedEmail = normalizedEmail;
            return Task.FromResult<object>(null);
        }

        public Task AddLoginAsync(ApplicationUser user,
            UserLoginInfo login, CancellationToken cancellationToken)
        {
            var temp1 = user ?? throw new ArgumentNullException(nameof(user));
            var temp2 = login ?? throw new ArgumentNullException(nameof(login));

            _context.UserLogins.Add(new ApplicationUserLogin()
            {
                Id = IdGenerator.GenerateObjectId().ToString(),
                LoginProvider = login.LoginProvider,
                ProviderDisplayName = login.ProviderDisplayName,
                ProviderKey = login.ProviderKey,
                UserId = user.Id,
            });

            return _context.SaveChangesAsync(cancellationToken);
        }

        public Task RemoveLoginAsync(ApplicationUser user,
            string loginProvider, string providerKey,
            CancellationToken cancellationToken)
        {
            var temp1 = user ?? throw new ArgumentNullException(nameof(user));
            var temp2 = loginProvider ?? throw new ArgumentNullException(nameof(loginProvider));
            var temp3 = providerKey ?? throw new ArgumentNullException(nameof(providerKey));

            return _context.UserLogins.FirstOrDefaultAsync(
                m => m.UserId.Equals(user.Id, StringComparison.InvariantCultureIgnoreCase)
                && m.LoginProvider.Equals(loginProvider, StringComparison.InvariantCultureIgnoreCase)
                && m.ProviderKey.Equals(providerKey, StringComparison.InvariantCultureIgnoreCase))
                .ContinueWith(m2 =>
                {
                    if (m2.Result != null)
                    {
                        _context.UserLogins.Remove(m2.Result);
                        _context.SaveChanges();
                    }
                });
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(
            ApplicationUser user, CancellationToken cancellationToken)
        {
            return _context.UserLogins.Where(m => m.UserId.Equals(
                user.Id, StringComparison.InvariantCultureIgnoreCase))
                .ToListAsync()
                .ContinueWith(m2 =>
                {
                    IList<UserLoginInfo> tempList = new List<UserLoginInfo>();

                    if (m2.Result != null)
                    {
                        m2.Result.ForEach(m1 =>
                        {
                            UserLoginInfo info = new UserLoginInfo(m1.LoginProvider,
                                m1.ProviderKey, m1.ProviderDisplayName);
                            tempList.Add(info);
                        });
                    }

                    return tempList;
                }
                );
        }

        public Task<ApplicationUser> FindByLoginAsync(
            string loginProvider, string providerKey,
            CancellationToken cancellationToken)
        {
            var temp2 = loginProvider ?? throw new ArgumentNullException(nameof(loginProvider));
            var temp3 = providerKey ?? throw new ArgumentNullException(nameof(providerKey));

            return _context.UserLogins.FirstOrDefaultAsync(
                m => m.LoginProvider.Equals(loginProvider, StringComparison.InvariantCultureIgnoreCase)
                && m.ProviderKey.Equals(providerKey, StringComparison.InvariantCultureIgnoreCase))
                .ContinueWith<ApplicationUser>(m2 =>
                {
                    if (m2.Result != null && !string.IsNullOrEmpty(m2.Result.UserId))
                    {
                        return _context.Users.Find(m2.Result.UserId);
                    }

                    return null;
                });
        }

        public Task SetPasswordHashAsync(ApplicationUser user,
            string passwordHash, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            if (string.IsNullOrEmpty(passwordHash))
            {
                throw new ArgumentNullException(nameof(passwordHash), "Parameter passwordHash cannot be null or empty.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            user.PasswordHash = passwordHash;
            return Task.FromResult<object>(null);
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user,
            CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user,
            CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task SetPhoneNumberAsync(ApplicationUser user,
            string phoneNumber, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            user.PhoneNumber = phoneNumber;
            return Task.FromResult<object>(null);
        }

        public Task<string> GetPhoneNumberAsync(ApplicationUser user,
            CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(
            ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(ApplicationUser user,
            bool confirmed, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            user.PhoneNumberConfirmed = confirmed;
            return Task.FromResult<object>(null);
        }

        public Task SetTwoFactorEnabledAsync(ApplicationUser user,
            bool enabled, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            user.TwoFactorEnabled = enabled;
            return Task.FromResult<object>(null);
        }

        public Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user,
            CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task SetSecurityStampAsync(ApplicationUser user,
            string stamp, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            user.SecurityStamp = stamp;
            return Task.FromResult<object>(null);
        }

        public Task<string> GetSecurityStampAsync(ApplicationUser user,
            CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.SecurityStamp);
        }

        public Task<IList<Claim>> GetClaimsAsync(ApplicationUser user,
            CancellationToken cancellationToken)
        {
            var temp = user ?? throw new ArgumentNullException(nameof(user));
            return _context.UserClaims.Where(m => m.UserId.Equals(user.Id))
                .ToListAsync(cancellationToken)
                .ContinueWith<IList<Claim>>(m2 =>
                {
                    IList<Claim> tempList = new List<Claim>();
                    m2.Result.ForEach(m1 =>
                    {
                        tempList.Add(new Claim(m1.ClaimType, m1.ClaimValue));
                    });
                    return tempList;
                }, cancellationToken);
        }

        public Task AddClaimsAsync(ApplicationUser user,
            IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            var temp = user ?? throw new ArgumentNullException(nameof(user));
            if (claims != null)
            {
                var temp2 = from one in claims
                            select new ApplicationUserClaim()
                            {
                                Id = IdGenerator.GenerateObjectId().ToString(),
                                UserId = user.Id,
                                ClaimType = one.Type,
                                ClaimValue = one.Value,
                            };

                _context.UserClaims.AddRange(temp2);
            }
            return _context.SaveChangesAsync(cancellationToken);
        }

        public Task ReplaceClaimAsync(ApplicationUser user,
            Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            var temp1 = user ?? throw new ArgumentNullException(nameof(user));
            var temp2 = user ?? throw new ArgumentNullException(nameof(claim));
            var temp3 = user ?? throw new ArgumentNullException(nameof(newClaim));

            return _context.UserClaims.FirstOrDefaultAsync(m =>
            m.ClaimType.Equals(claim.Type, StringComparison.InvariantCultureIgnoreCase)
            && m.ClaimValue.Equals(claim.Value, StringComparison.InvariantCultureIgnoreCase)
            && m.UserId.Equals(user.Id, StringComparison.InvariantCultureIgnoreCase))
            .ContinueWith(m2 =>
            {
                m2.Result.ClaimType = newClaim.Type;
                m2.Result.ClaimValue = newClaim.Value;

                _context.SaveChanges();

            }, cancellationToken);
        }

        public Task RemoveClaimsAsync(ApplicationUser user,
            IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            var temp = user ?? throw new ArgumentNullException(nameof(user));
            return _context.UserClaims.Where(m => m.UserId.Equals(user.Id))
                .ToListAsync(cancellationToken)
                .ContinueWith(m2 =>
                {
                    if (m2.Result != null && claims != null)
                    {
                        foreach (var m1 in m2.Result)
                        {
                            if (claims.Any(m3 => m3.Type.Equals(
                                 m1.ClaimType, StringComparison.InvariantCultureIgnoreCase)
                                 && m3.Value.Equals(
                                 m1.ClaimValue, StringComparison.InvariantCultureIgnoreCase)))
                            {
                                _context.UserClaims.Remove(m1);
                            }
                        }
                        _context.SaveChanges();
                    }
                }, cancellationToken);
        }

        public Task<IList<ApplicationUser>> GetUsersForClaimAsync(
            Claim claim, CancellationToken cancellationToken)
        {
            var temp = claim ?? throw new ArgumentNullException(nameof(claim));
            return _context.Users.Join(_context.UserClaims,
                outer => outer.Id, inner => inner.UserId, (User, Claim) => new { User, Claim })
                .Where(m => m.Claim.ClaimType.Equals(
                                  claim.Type, StringComparison.InvariantCultureIgnoreCase)
                                  && m.Claim.ClaimValue.Equals(claim.Value,
                                  StringComparison.InvariantCultureIgnoreCase))
                                  .ToListAsync(cancellationToken)
                                  .ContinueWith(m1 =>
                                  {
                                      IList<ApplicationUser> users = null;
                                      if (m1.Result != null)
                                      {
                                          var userDic = new Dictionary<string, ApplicationUser>();
                                          m1.Result.ForEach(m2 =>
                                          {
                                              if (!userDic.ContainsKey(m2.User.Id))
                                                  userDic.Add(m2.User.Id, m2.User);
                                          });

                                          users = userDic.Values.ToList();
                                      }
                                      return users;
                                  });
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(
            ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.LockoutEnd ?? null);
        }

        public Task SetLockoutEndDateAsync(ApplicationUser user,
            DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            user.LockoutEnd = lockoutEnd;
            //user.LockoutEndDateTimeUtc = lockoutEnd?.UtcDateTime;
            return Task.FromResult<object>(null);
        }

        public Task<int> IncrementAccessFailedCountAsync(
            ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(
            ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            user.AccessFailedCount = 0;
            return Task.FromResult<object>(null);
        }

        public Task<int> GetAccessFailedCountAsync(ApplicationUser user,
            CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(ApplicationUser user,
            CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(ApplicationUser user,
            bool enabled, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Parameter user is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            user.LockoutEnabled = enabled;
            return Task.FromResult<object>(null);
        }

        public Task AddToRoleAsync(ApplicationUser user,
            string roleName, CancellationToken cancellationToken)
        {
            var temp = user ?? throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrEmpty(roleName))
                return Task.FromResult(new object());
            return _context.Roles.FirstOrDefaultAsync(m => m.NormalizedName.Equals(
                roleName, StringComparison.InvariantCultureIgnoreCase))
                .ContinueWith(m =>
                {
                    if (m.Result != null)
                    {
                        ApplicationUserRole userRole = new ApplicationUserRole()
                        {
                            Id = IdGenerator.GenerateObjectId().ToString(),
                            UserId = user.Id,
                            RoleId = m.Result.Id,
                        };
                        _context.Add(userRole);
                        _context.SaveChanges();
                    }
                }, cancellationToken);
        }

        public Task RemoveFromRoleAsync(ApplicationUser user,
            string roleName, CancellationToken cancellationToken)
        {
            var temp = user ?? throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrEmpty(roleName))
                return Task.FromResult(new object());
            return _context.Roles.FirstOrDefaultAsync(m => m.NormalizedName.Equals(
                roleName, StringComparison.InvariantCultureIgnoreCase))
                .ContinueWith(m =>
                {
                    if (m.Result != null)
                    {
                        var userRole = _context.UserRoles.FirstOrDefault(
                            m2 => m.Result.Id.Equals(m2.RoleId, StringComparison.InvariantCultureIgnoreCase)
                            && m2.UserId.Equals(user.Id));

                        _context.UserRoles.Remove(userRole);
                        _context.SaveChanges();
                    }
                }, cancellationToken);
        }

        public Task<IList<string>> GetRolesAsync(ApplicationUser user,
            CancellationToken cancellationToken)
        {
            var temp = user ?? throw new ArgumentNullException(nameof(user));
            IList<string> roleIds = null;

            return _context.UserRoles.Where(m => m.UserId.Equals(
                user.Id, StringComparison.InvariantCultureIgnoreCase))
                .Select(m => m.RoleId)
                .ToArrayAsync().ContinueWith(m =>
                {
                    roleIds = new List<string>(m.Result);
                    return roleIds;
                });
        }

        public Task<bool> IsInRoleAsync(ApplicationUser user,
            string roleName, CancellationToken cancellationToken)
        {
            var temp = user ?? throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrEmpty(roleName))
                return Task.FromResult(false);
            return _context.Roles.FirstOrDefaultAsync(m => m.NormalizedName.Equals(
                roleName, StringComparison.InvariantCultureIgnoreCase))
                .ContinueWith<bool>(m =>
                {
                    if (m.Result != null)
                    {
                        return _context.UserRoles.Any(m1 => m.Result.Id.Equals(m1.RoleId,
                            StringComparison.InvariantCultureIgnoreCase)
                            && m1.UserId.Equals(user.Id));
                    }

                    return false;
                }, cancellationToken);
        }

        public Task<IList<ApplicationUser>> GetUsersInRoleAsync(
            string roleName, CancellationToken cancellationToken)
        {
            IList<ApplicationUser> users = null;
            if (string.IsNullOrEmpty(roleName))
            {
                users = new List<ApplicationUser>();
                return Task.FromResult(users);
            }

            return _context.Users.Join(_context.UserRoles.Join(_context.Roles,
                outer => outer.RoleId, inner => inner.Id, (UserRole, Role) => new { Role, UserRole }),
                outer => outer.Id, inner => inner.UserRole.UserId,
                (User, UserRoleRelation) => new { User, UserRoleRelation })
                .Where(m => m.UserRoleRelation.Role.NormalizedName.Equals(
                                  roleName, StringComparison.InvariantCultureIgnoreCase))
                                  .ToListAsync(cancellationToken)
                                  .ContinueWith(m1 =>
                                  {
                                      if (m1.Result != null)
                                      {
                                          var userDic = new Dictionary<string, ApplicationUser>();
                                          m1.Result.ForEach(m2 =>
                                          {
                                              if (!userDic.ContainsKey(m2.User.Id))
                                                  userDic.Add(m2.User.Id, m2.User);
                                          });

                                          users = userDic.Values.ToList();
                                      }
                                      return users;
                                  });
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        public IQueryable<ApplicationUser> Users => throw new NotImplementedException();

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    logger.LogTrace("EfCoreUserStore.ApplicationDbContext Dispose");
                    this._context?.Dispose();
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~EfCoreRoleStore() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
