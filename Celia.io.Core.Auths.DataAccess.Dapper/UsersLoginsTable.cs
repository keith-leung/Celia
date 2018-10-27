using Celia.io.Core.Auths.Abstractions;
using Celia.io.Core.Utils;
using Dapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Celia.io.Core.Auths.DataAccess.Dapper
{
    public class UsersLoginsTable : IDisposable
    {
        private readonly IDbConnection _connection;

        public UsersLoginsTable(IDbConnection conn)
        {
            _connection = conn ?? throw new ArgumentNullException(
                nameof(conn));
        }

        public Task AddLoginAsync(ApplicationUser user, UserLoginInfo login)
        {
            const string command = "INSERT INTO auths_user_logins(Id, LoginProvider, ProviderKey, UserId, ProviderDisplayName) " +
                                   "VALUES (@Id, @LoginProvider, @ProviderKey, @UserId, @ProviderDisplayName) " +
                                   "ON DUPLICATE KEY UPDATE LoginProvider=@LoginProvider, ProviderKey=@ProviderKey," +
                                   " UserId=@UserId, ProviderDisplayName=@ProviderDisplayName ;";

            return _connection.ExecuteAsync(command, new
            {
                Id = HashUtils.GetMd5String($"{user.Id}___{login.LoginProvider}___{login.ProviderKey}"),
                LoginProvider = login.LoginProvider,
                ProviderKey = login.ProviderKey,
                UserId = user.Id,
                ProviderDisplayName = login.ProviderDisplayName
            });
        }

        public Task RemoveLoginAsync(ApplicationUser user, string loginProvider, string providerKey)
        {
            const string command = "DELETE " +
                                   "FROM auths_user_logins " +
                                   "WHERE UserId = @UserId AND LoginProvider = @LoginProvider AND ProviderKey = @ProviderKey;";

            return _connection.ExecuteAsync(command, new
            {
                UserId = user.Id,
                LoginProvider = loginProvider,
                ProviderKey = providerKey
            });
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            const string command = "SELECT * " +
                                   "FROM auths_user_logins " +
                                   "WHERE UserId = @UserId;";

            IEnumerable<ApplicationUserLogin> userLogins = Task.Run(() => _connection.QueryAsync<ApplicationUserLogin>(command, new
            {
                UserId = user.Id
            }), cancellationToken).Result;

            return Task.FromResult<IList<UserLoginInfo>>(userLogins.Select(e => new UserLoginInfo(e.LoginProvider, e.ProviderKey, e.ProviderDisplayName)).ToList());
        }

        public Task<ApplicationUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            string[] command =
            {
                "SELECT UserId " +
                "FROM auths_user_logins " +
                "WHERE LoginProvider = @LoginProvider AND ProviderKey = @ProviderKey;"
            };

            string userId = Task.Run(() =>
            _connection.QuerySingleOrDefaultAsync<string>(
                command[0], new
                {
                    LoginProvider = loginProvider,
                    ProviderKey = providerKey
                }), cancellationToken).Result;

            if (string.IsNullOrEmpty(userId))
            {
                return Task.FromResult<ApplicationUser>(null);
            }

            command[0] = "SELECT * " +
                         "FROM auths_users " +
                         "WHERE Id = @Id;";

            return _connection.QuerySingleAsync<ApplicationUser>(
                command[0], new { Id = userId });
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _connection?.Dispose();
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~UsersLoginsTable() {
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
