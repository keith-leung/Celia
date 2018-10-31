using Celia.io.Core.Auths.Abstractions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Claims;
using Celia.io.Core.Utils;
using System.Linq;

namespace Celia.io.Core.Auths.DataAccess.Dapper
{
    public class RolesTable : IDisposable
    {
        private IDbConnection _connection;

        public RolesTable(IDbConnection conn)
        {
            this._connection = conn ?? throw new ArgumentNullException(
                 nameof(conn));
        }

        public Task<IdentityResult> CreateAsync(ApplicationRole role,
            CancellationToken cancellationToken)
        {
            string tempCommand = "SELECT 1 FROM auths_roles WHERE NormalizedName = @NormalizedName ";
            int result1 = _connection.ExecuteScalar<int>(tempCommand,
                new
                {
                    NormalizedName = role.NormalizedName
                });
            //保证幂等，因为MySQL不支持Insert where，只能在API层尝试
            if (result1 > 0)
                return Task.FromResult(IdentityResult.Success);

            const string command = "INSERT INTO auths_roles(Id, ConcurrencyStamp, Name, NormalizedName)  " +
                                   "VALUES (@Id, @ConcurrencyStamp, @Name, @NormalizedName);";

            int rowsInserted = Task.Run(() => _connection.ExecuteAsync(command, new
            {
                role.Id,
                role.ConcurrencyStamp,
                role.Name,
                role.NormalizedName
            }), cancellationToken).Result;

            return Task.FromResult(rowsInserted.Equals(1) ? IdentityResult.Success : IdentityResult.Failed(new IdentityError
            {
                Code = string.Empty,
                Description = $"The role with name {role.Name} could not be inserted in the auths_roles table."
            }));
        }

        public Task<IdentityResult> UpdateAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            const string command = "UPDATE auths_roles " +
                                   "SET ConcurrencyStamp = @ConcurrencyStamp, Name = @Name, NormalizedName = @NormalizedName " +
                                   "WHERE Id = @Id;";

            return Task.Run(() => _connection.ExecuteAsync(command, new
            {
                role.ConcurrencyStamp,
                role.Name,
                role.NormalizedName,
                role.Id
            }), cancellationToken).ContinueWith(o => IdentityResult.Success);

            /*
            : IdentityResult.Failed(new IdentityError
            {
                Code = string.Empty,
                Description = $"The role with name {role.Name} could not be updated in the auths_roles table."
            }));*/
        }

        public Task<IdentityResult> DeleteAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            const string command = "DELETE " +
                                   "FROM auths_roles " +
                                   "WHERE Id = @Id;";

            return Task.Run(() => _connection.ExecuteAsync(command, new { role.Id }),
                cancellationToken)
                .ContinueWith(m => IdentityResult.Success);

            //return Task.FromResult(IdentityResult.Success);
            //: IdentityResult.Failed(new IdentityError
            //{
            //    Code = string.Empty,
            //    Description = $"The role with name {role.Name} could not be deleted from the auths_roles table."
            //}));
        }

        public Task<ApplicationRole> FindByIdAsync(string roleId)
        {
            const string command = "SELECT Id,NormalizedName,Name,ConcurrencyStamp " +
                                   "FROM auths_roles " +
                                   "WHERE Id = @Id;";

            return _connection.QuerySingleOrDefaultAsync<ApplicationRole>(command, new
            {
                Id = roleId
            });
        }

        public Task<ApplicationRole> FindByNameAsync(string normalizedRoleName)
        {
            const string command = "SELECT * " +
                                   "FROM auths_roles " +
                                   "WHERE NormalizedName = @NormalizedName;";

            return _connection.QuerySingleOrDefaultAsync<ApplicationRole>(command, new
            {
                NormalizedName = normalizedRoleName
            });
        }

        public Task<IQueryable<ApplicationRole>> GetAllRoles()
        {
            const string command = "SELECT * " +
                                   "FROM auths_roles;";

            return _connection.QueryAsync<ApplicationRole>(command)
                .ContinueWith(a =>
                {
                    if (a.IsCompleted && a.Result != null)
                        return a.Result.AsQueryable();
                    return (new List<ApplicationRole>()).AsQueryable();
                });
        }

        public Task<IList<Claim>> GetClaimsAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            const string command = "SELECT * " +
                                   "FROM auths_role_claims WHERE RoleId = @RoleId;";

            return _connection.QueryAsync<ApplicationRoleClaim>(command,
                    new
                    {
                        RoleId = role.Id
                    }
                )
                .ContinueWith<IList<Claim>>(action =>
                {
                    List<Claim> claims = new List<Claim>();
                    if (action.IsCompleted && !action.IsFaulted && action.Result != null)
                    {
                        foreach (var c in action.Result)
                        {
                            claims.Add(c.ToClaim());
                        }
                    }

                    return claims;
                });
        }

        public Task AddClaimAsync(ApplicationRole role, Claim claim, CancellationToken cancellationToken)
        {
            const string command = "INSERT INTO auths_role_claims(" +
                "Id, RoleId, ClaimType, ClaimValue) " +
                                   "VALUES (@Id, @RoleId, @ClaimType, @ClaimValue) " +
                                   "ON DUPLICATE KEY UPDATE RoleId = @RoleId, " +
                                   "ClaimType = @ClaimType, ClaimValue = @ClaimValue ;";

            return _connection.ExecuteAsync(command, new
            {
                Id = HashUtils.GetMd5String($"{role.Id}___{claim.Type}___{claim.Value}"),
                RoleId = role.Id,
                ClaimType = claim.Type,
                ClaimValue = claim.Value,
            });
        }

        public Task RemoveClaimAsync(ApplicationRole role, Claim claim, CancellationToken cancellationToken)
        {
            const string command = "DELETE " +
                                   "FROM auths_role_claims " +
                                   "WHERE RoleId = @RoleId " +
                                   "AND ClaimType = @ClaimType " +
                                   " AND ClaimValue = @ClaimValue;";

            return _connection.ExecuteAsync(command, new
            {
                RoleId = role.Id,
                ClaimType = claim.Type,
                ClaimValue = claim.Value,
            });
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_connection == null)
                    {
                        return;
                    }

                    _connection.Dispose();
                    _connection = null;
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~RolesTable() {
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
