using Celia.io.Core.Auths.Abstractions;
using Celia.io.Core.Utils;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Celia.io.Core.Auths.DataAccess.Dapper
{
    public class UsersRolesTable : IDisposable
    {
        private readonly IDbConnection _connection;

        public UsersRolesTable(IDbConnection conn)
        {
            _connection = conn ?? throw new ArgumentNullException(
                nameof(conn));
        }

        public Task AddToRoleAsync(ApplicationUser user, string roleId)
        {
            const string command = "INSERT INTO auths_user_roles(Id, UserId, RoleId) " +
                                   "VALUES (@Id, @UserId, @RoleId) " +
                                   "ON DUPLICATE KEY UPDATE UserId=@UserId, RoleId=@RoleId ;";

            return _connection.ExecuteAsync(command, new
            {
                Id = HashUtils.GetMd5String($"{user.Id}___{roleId}"),
                UserId = user.Id,
                RoleId = roleId
            });
        }

        public Task RemoveFromRoleAsync(ApplicationUser user, string roleId)
        {
            const string command = "DELETE " +
                                   "FROM auths_user_roles " +
                                   "WHERE UserId = @UserId AND RoleId = @RoleId;";

            return _connection.ExecuteAsync(command, new
            {
                UserId = user.Id,
                RoleId = roleId
            });
        }

        public Task<IList<string>> GetRolesAsync(ApplicationUser user,
            CancellationToken cancellationToken)
        {
            const string command = "SELECT r.Name " +
                                   "FROM auths_roles as r " +
                                   "INNER JOIN auths_user_roles AS ur ON ur.RoleId = r.Id " +
                                   "WHERE ur.UserId = @UserId;";

            IEnumerable<string> userRoles = Task.Run(
                () => _connection.QueryAsync<string>(command, new
                {
                    UserId = user.Id
                }), cancellationToken).Result;

            return Task.FromResult<IList<string>>(userRoles.ToList());
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    this._connection?.Dispose();
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~UsersRolesTable() {
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
