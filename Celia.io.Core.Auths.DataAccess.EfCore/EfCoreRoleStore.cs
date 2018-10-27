using Celia.io.Core.Auths.Abstractions;
using Celia.io.Core.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Celia.io.Core.Auths.DataAccess.EfCore
{
    public class EfCoreRoleStore : IApplicationRoleStore
    {
        private readonly ApplicationDbContext _context = null;
        private readonly ILogger logger = null;

        public EfCoreRoleStore(ILogger<EfCoreRoleStore> logger, ApplicationDbContext context)
        {
            this._context = context ?? throw new ArgumentNullException(
                nameof(context));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<IdentityResult> CreateAsync(
            ApplicationRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role),
                    "Parameter role is not set to an instance of an object.");
            }
            _context.Roles.Add(role);

            return _context.SaveChangesAsync(cancellationToken)
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
        }

        public Task<IdentityResult> DeleteAsync(
            ApplicationRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role),
                    "Parameter role is not set to an instance of an object.");
            }

            _context.Roles.Remove(role);

            return _context.SaveChangesAsync(cancellationToken)
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
        }

        public Task<ApplicationRole> FindByIdAsync(
            string roleId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(roleId))
                return Task.FromResult<ApplicationRole>(null);
            return _context.Roles.FindAsync(new object[] { roleId }, cancellationToken);
        }

        public Task<ApplicationRole> FindByNameAsync(
            string normalizedRoleName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(normalizedRoleName))
                return Task.FromResult<ApplicationRole>(null);

            return _context.Roles.FirstOrDefaultAsync(
                m => !string.IsNullOrEmpty(m.NormalizedName)
                && m.NormalizedName.Equals(normalizedRoleName),
                cancellationToken);
        }

        public Task<string> GetNormalizedRoleNameAsync(
            ApplicationRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role), "Parameter role is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync(
            ApplicationRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role),
                    "Parameter role is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(role.Id.ToString());
        }

        public Task<string> GetRoleNameAsync(ApplicationRole role,
            CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role),
                    "Parameter role is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(
            ApplicationRole role, string normalizedName,
            CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role),
                    "Parameter role is not set to an instance of an object.");
            }

            if (string.IsNullOrEmpty(normalizedName))
            {
                throw new ArgumentNullException(nameof(normalizedName),
                    "Parameter normalizedName cannot be null or empty.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            role.NormalizedName = normalizedName;
            return Task.FromResult<object>(null);
        }

        public Task SetRoleNameAsync(ApplicationRole role, string roleName,
            CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role),
                    "Parameter role is not set to an instance of an object.");
            }

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException(nameof(roleName),
                    "Parameter roleName cannot be null or empty.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            role.Name = roleName;
            return Task.FromResult<object>(null);
        }

        public Task<IdentityResult> UpdateAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role),
                    "Parameter role is not set to an instance of an object.");
            }
            return this._context.Roles.FindAsync(new object[] { role.Id },
                cancellationToken)
                .ContinueWith<IdentityResult>((u) =>
                {
                    if (u.IsCanceled)
                        return IdentityResult.Failed(
                            new IdentityError()
                            {
                                Code = "400",
                                Description = "User canceled"
                            });

                    if (u.IsCompleted && u.Result != null)
                    {
                        u.Wait();

                        if (!string.IsNullOrEmpty(role.ConcurrencyStamp)
                            && !string.IsNullOrEmpty(u.Result.ConcurrencyStamp)
                            && string.Equals(u.Result.ConcurrencyStamp,
                            role.ConcurrencyStamp, StringComparison.InvariantCultureIgnoreCase))
                        {
                            u.Result.Name = role.Name;
                            u.Result.NormalizedName = role.NormalizedName;
                            if (string.IsNullOrEmpty(role.ConcurrencyStamp))
                            {
                                u.Result.ConcurrencyStamp = Guid.NewGuid().ToString();
                            }
                            else
                            {
                                u.Result.ConcurrencyStamp = role.ConcurrencyStamp;
                            }
                        }

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

        #region IRoleClaimStore
        public Task<IList<Claim>> GetClaimsAsync(ApplicationRole role,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role),
                    "Parameter role is not set to an instance of an object.");
            }
            IList<Claim> claims = null;
            return this._context.RoleClaims.Where(
                m => m.RoleId.Equals(role.Id, StringComparison.InvariantCultureIgnoreCase))
                .ToListAsync(cancellationToken)
                .ContinueWith<IList<Claim>>(m2 =>
                {
                    if (m2.IsCompleted && m2.Result != null)
                    {
                        claims = new List<Claim>();
                        m2.Result.ForEach(m1 =>
                        {
                            claims.Add(new Claim(m1.ClaimType, m1.ClaimValue));
                        });
                    }

                    return claims;
                });
        }

        public Task AddClaimAsync(ApplicationRole role, Claim claim,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role),
                    "Parameter role is not set to an instance of an object.");
            }
            if (claim == null)
            {
                return Task.FromResult(new object());
            }

            this._context.RoleClaims.Add(new ApplicationRoleClaim()
            {
                Id = IdGenerator.GenerateObjectId().ToString(),
                RoleId = role.Id,
                ClaimType = claim.Type,
                ClaimValue = claim.Value
            });

            return this._context.SaveChangesAsync(cancellationToken);
        }

        public Task RemoveClaimAsync(ApplicationRole role, Claim claim,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role),
                    "Parameter role is not set to an instance of an object.");
            }
            if (claim == null)
            {
                return Task.FromResult(new object());
            }

            var entity = _context.RoleClaims.FirstOrDefault(
                m => m.RoleId.Equals(role.Id, StringComparison.InvariantCultureIgnoreCase)
                && m.ClaimType.Equals(claim.Type, StringComparison.InvariantCultureIgnoreCase)
                && m.ClaimValue.Equals(claim.Value, StringComparison.InvariantCultureIgnoreCase));

            if (entity != null)
            {
                _context.RoleClaims.Remove(entity);
                return _context.SaveChangesAsync(cancellationToken);
            }

            return Task.FromResult(new object());
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        public IQueryable<ApplicationRole> Roles
        {
            get
            {
                return _context.Roles;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    logger.LogTrace("EfCoreRoleStore.ApplicationDbContext Dispose");
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