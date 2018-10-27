using Celia.io.Core.Auths.Abstractions;
using Celia.io.Core.Utils;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Celia.io.Core.Auths.DataAccess.Dapper
{
    public class DapperRoleStore : IApplicationRoleStore
    {
        public DapperRoleStore(IDbConnection conn)
        {
            var temp = conn ?? throw new ArgumentNullException(nameof(conn));
            _rolesTable = new RolesTable(temp);
        }

        private readonly RolesTable _rolesTable;

        public IQueryable<ApplicationRole> Roles
        {
            get
            {
                return _rolesTable.GetAllRoles().Result;
            }
        }

        public Task<IdentityResult> CreateAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role), "Parameter role is not set to an instance of an object.");
            }

            if (string.IsNullOrEmpty(role.Id))
            {
                role.Id = IdGenerator.GenerateObjectId().ToString();
            }

            return _rolesTable.CreateAsync(role, cancellationToken);
        }

        public Task<IdentityResult> UpdateAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role), "Parameter role is not set to an instance of an object.");
            }

            return _rolesTable.UpdateAsync(role, cancellationToken);
        }

        public Task<IdentityResult> DeleteAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role), "Parameter role is not set to an instance of an object.");
            }

            return _rolesTable.DeleteAsync(role, cancellationToken);
        }

        public Task<string> GetRoleIdAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role), "Parameter role is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(role.Id);
        }

        public Task<string> GetRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role), "Parameter role is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(role.Name);
        }

        public Task SetRoleNameAsync(ApplicationRole role, string roleName, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role), "Parameter role is not set to an instance of an object.");
            }

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException(nameof(roleName), "Parameter roleName cannot be null or empty.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            role.Name = roleName;
            return Task.FromResult<object>(null);
        }

        public Task<string> GetNormalizedRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role), "Parameter role is not set to an instance of an object.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(role.NormalizedName);
        }

        public Task SetNormalizedRoleNameAsync(ApplicationRole role, string normalizedName, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role), "Parameter role is not set to an instance of an object.");
            }

            if (string.IsNullOrEmpty(normalizedName))
            {
                throw new ArgumentNullException(nameof(normalizedName), "Parameter normalizedName cannot be null or empty.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            role.NormalizedName = normalizedName;
            return Task.FromResult<object>(null);
        }

        public Task<ApplicationRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                throw new ArgumentNullException(nameof(roleId), "Parameter roleId cannot be null or empty.");
            }

            //bool isValidGuid = Guid.TryParse(roleId, out Guid roleGuid);

            //if (!isValidGuid)
            //{
            //    throw new ArgumentException("Parameter roleId is not a valid Guid.", nameof(roleId));
            //}

            cancellationToken.ThrowIfCancellationRequested();
            return _rolesTable.FindByIdAsync(roleId);
        }

        public Task<ApplicationRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(normalizedRoleName))
            {
                throw new ArgumentNullException(nameof(normalizedRoleName), "Parameter normalizedRoleName cannot be null or empty.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            return _rolesTable.FindByNameAsync(normalizedRoleName);
        }

        #region ApplicationRoleClaim
        public Task<IList<Claim>> GetClaimsAsync(ApplicationRole role,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role),
                    "Parameter role is not set to an instance of an object.");
            }

            return _rolesTable.GetClaimsAsync(role, cancellationToken);
        }

        public Task AddClaimAsync(ApplicationRole role, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role),
                    "Parameter role is not set to an instance of an object.");
            }
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            return _rolesTable.AddClaimAsync(role, claim, cancellationToken);
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
                throw new ArgumentNullException(nameof(claim));
            }

            return _rolesTable.RemoveClaimAsync(role, claim, cancellationToken);
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this._rolesTable.Dispose();
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~DapperRoleStore() {
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
