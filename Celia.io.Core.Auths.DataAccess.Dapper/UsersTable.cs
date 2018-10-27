using Celia.io.Core.Auths.Abstractions;
using Dapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Celia.io.Core.Auths.DataAccess.Dapper
{
    public class UsersTable : IDisposable
    {
        private readonly IDbConnection _connection;
        public UsersTable(IDbConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(
                nameof(connection));
        }

        public Task<IdentityResult> CreateAsync(ApplicationUser user,
            CancellationToken cancellationToken)
        {
            string tempCommand = "SELECT 1 FROM auths_users WHERE NormalizedUserName = @NormalizedUserName " +
                "OR NormalizedEmail = @NormalizedEmail ";
            int result1 = _connection.ExecuteScalar<int>(tempCommand,
                new
                {
                    NormalizedUserName = user.NormalizedUserName,
                    NormalizedEmail = user.NormalizedEmail,
                });
            //保证幂等，因为MySQL不支持Insert where，只能在API层尝试
            if (result1 > 0)
                return Task.FromResult(IdentityResult.Success);

            const string command = "INSERT INTO auths_users(Id,UserName," +
                " NormalizedUserName,Email,NormalizedEmail," +
                " EmailConfirmed,PasswordHash,PhoneNumber,PhoneNumberConfirmed," +
                " ConcurrencyStamp,SecurityStamp,LockoutEnabled," +
                " TwoFactorEnabled,AccessFailedCount,LockoutEnd) " +
                                   "VALUES (@Id, @UserName, @NormalizedUserName, " +
                                   " @Email, @NormalizedEmail, @EmailConfirmed, " +
                                           "@PasswordHash, @PhoneNumber, @PhoneNumberConfirmed, " +
                                           "@ConcurrencyStamp, @SecurityStamp, " +
                                           "@LockoutEnabled, @TwoFactorEnabled, " +
                                           "@AccessFailedCount, @LockoutEnd) ;";

            int rowsInserted = Task.Run(() => _connection.ExecuteAsync(command, new
            {
                Id = user.Id,
                UserName = user.UserName,
                NormalizedUserName = user.NormalizedUserName,
                Email = user.Email,
                NormalizedEmail = user.NormalizedEmail,
                EmailConfirmed = user.EmailConfirmed,
                PasswordHash = user.PasswordHash,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                ConcurrencyStamp = user.ConcurrencyStamp,
                SecurityStamp = user.SecurityStamp,
                LockoutEnabled = user.LockoutEnabled,
                TwoFactorEnabled = user.TwoFactorEnabled,
                AccessFailedCount = user.AccessFailedCount,
                LockoutEnd = user.LockoutEnd
            }), cancellationToken).Result;

            return Task.FromResult(rowsInserted.Equals(1) ? IdentityResult.Success : IdentityResult.Failed(new IdentityError
            {
                Code = string.Empty,
                Description = $"The user with email {user.Email} could not be inserted in the auths_users table."
            }));
        }

        public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            const string command = "DELETE " +
                                   "FROM auths_users " +
                                   "WHERE Id = @Id;";

            return Task.Run(() => _connection.ExecuteAsync(command, new
            {
                user.Id
            }), cancellationToken)
            .ContinueWith(m => IdentityResult.Success);

            //    return Task.FromResult(rowsDeleted.Equals(1) ? IdentityResult.Success : IdentityResult.Failed(new IdentityError
            //    {
            //        Code = string.Empty,
            //        Description = $"The user with email {user.Email} could not be deleted from the auths_users table."
            //    }));
        }

        public Task<ApplicationUser> FindByIdAsync(string userId)
        {
            const string command = "SELECT * " +
                                   "FROM auths_users " +
                                   "WHERE Id = @Id;";

            return _connection.QuerySingleOrDefaultAsync<ApplicationUser>(command, new
            {
                Id = userId
            });
        }

        public Task<ApplicationUser> FindByNameAsync(string normalizedUserName)
        {
            const string command = "SELECT * " +
                                   "FROM auths_users " +
                                   "WHERE NormalizedUserName = @NormalizedUserName;";

            return _connection.QuerySingleOrDefaultAsync<ApplicationUser>(command, new
            {
                NormalizedUserName = normalizedUserName
            });
        }

        public Task<ApplicationUser> FindByEmailAsync(string normalizedEmail)
        {
            const string command = "SELECT * " +
                                   "FROM auths_users " +
                                   "WHERE NormalizedEmail = @NormalizedEmail;";

            return _connection.QuerySingleOrDefaultAsync<ApplicationUser>(command, new
            {
                NormalizedEmail = normalizedEmail
            });
        }

        public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            const string command = "UPDATE auths_users " +
                                   "SET UserName = @UserName, NormalizedUserName = @NormalizedUserName, " +
                                   " Email = @Email, NormalizedEmail = @NormalizedEmail, " +
                                       "EmailConfirmed = @EmailConfirmed, PasswordHash = @PasswordHash, " +
                                       "PhoneNumber = @PhoneNumber, PhoneNumberConfirmed = @PhoneNumberConfirmed, " +
                                       "ConcurrencyStamp = @ConcurrencyStamp, SecurityStamp = @SecurityStamp, " +
                                       "LockoutEnabled = @LockoutEnabled, " +
                                       "TwoFactorEnabled = @TwoFactorEnabled, AccessFailedCount = @AccessFailedCount, " +
                                       "LockoutEnd = @LockoutEnd " +
                                   "WHERE Id = @Id;";

            return Task.Run(() => _connection.ExecuteAsync(command, new
            {
                user.UserName,
                user.NormalizedUserName,
                user.Email,
                user.NormalizedEmail,
                user.EmailConfirmed,
                user.PasswordHash,
                user.PhoneNumber,
                user.PhoneNumberConfirmed,
                user.ConcurrencyStamp,
                user.SecurityStamp,
                user.LockoutEnabled,
                user.TwoFactorEnabled,
                user.AccessFailedCount,
                user.Id,
                user.LockoutEnd
            }), cancellationToken)
            .ContinueWith(o => IdentityResult.Success);
            //: IdentityResult.Failed(new IdentityError
            //{
            //    Code = string.Empty,
            //    Description = $"The user with email {user.Email} could not be updated in the auths_users table."
            //}));
        }

        public Task<IEnumerable<ApplicationUser>> GetAllUsers()
        {
            const string command = "SELECT * " +
                                   "FROM auths_users;";

            return _connection.QueryAsync<ApplicationUser>(command);
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this._connection?.Dispose();
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~UsersTable() {
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
