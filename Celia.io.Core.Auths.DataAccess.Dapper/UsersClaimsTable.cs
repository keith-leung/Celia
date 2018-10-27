using Celia.io.Core.Auths.Abstractions;
using Celia.io.Core.Utils;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Celia.io.Core.Auths.DataAccess.Dapper
{
    public class UsersClaimsTable : IDisposable
    {
        private readonly IDbConnection _connection;

        public UsersClaimsTable(IDbConnection conn)
        {
            _connection = conn ??
                throw new ArgumentNullException(nameof(conn));
        }

        public Task<IList<Claim>> GetClaimsAsync(
            ApplicationUser user, CancellationToken cancellationToken)
        {
            const string command = "SELECT * " +
                                   "FROM auths_user_claims " +
                                   "WHERE UserId = @UserId;";

            IEnumerable<ApplicationUserClaim> ApplicationUserClaims
                = Task.Run(() => _connection.QueryAsync<ApplicationUserClaim>(command, new
                {
                    UserId = user.Id
                }), cancellationToken).Result;

            return Task.FromResult<IList<Claim>>(
                ApplicationUserClaims.Select(e => new Claim(
                    e.ClaimType, e.ClaimValue)).ToList());
        }

        public Task AddClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims)
        {
            const string command = "INSERT INTO auths_user_claims " +
                                   "VALUES (@Id, @UserId, @ClaimType, @ClaimValue) " +
                                   "ON DUPLICATE KEY UPDATE UserId = @UserId, " +
                                   "ClaimType = @ClaimType, ClaimValue = @ClaimValue ; ";

            return _connection.ExecuteAsync(command, claims.Select(e => new
            {
                Id = HashUtils.GetMd5String($"{user.Id}___{e.Type}___{e.Value}"),
                UserId = user.Id,
                ClaimType = e.Type,
                ClaimValue = e.Value
            }));
        }

        public Task ReplaceClaimAsync(ApplicationUser user, Claim claim,
            Claim newClaim)
        {
            const string command = "UPDATE auths_user_claims " +
                                   "SET ClaimType = @NewClaimType, ClaimValue = @NewClaimValue " +
                                   "WHERE UserId = @UserId AND ClaimType = @ClaimType AND ClaimValue = @ClaimValue;";

            return _connection.ExecuteAsync(command, new
            {
                NewClaimType = newClaim.Type,
                NewClaimValue = newClaim.Value,
                UserId = user.Id,
                ClaimType = claim.Type,
                ClaimValue = claim.Value
            });
        }

        public Task RemoveClaimsAsync(ApplicationUser user,
            IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            const string command = "DELETE " +
                          "FROM auths_user_claims " +
                          "WHERE UserId = @UserId " +
                          "AND ClaimType = @ClaimType " +
                          "AND ClaimValue = @ClaimValue;";

            return Task.Run(() =>
            {
                //List<Task> tasks = new List<Task>();
                try
                {  
                    foreach (var c in claims)
                    {
                        _connection.Execute(command, new
                        {
                            UserId = user.Id,
                            ClaimType = c.Type,
                            ClaimValue = c.Value,
                        });
                        //tasks.Add(tk);
                    }

                    //Task.WaitAll(tasks.ToArray());
                }
                catch (Exception ex1)
                {
                    System.Console.WriteLine(ex1.Message);
                }
            }, cancellationToken);
        }

        public Task<IList<ApplicationUser>> GetUsersForClaimAsync(
            Claim claim, CancellationToken cancellationToken)
        {
            const string command = "SELECT DISTINCT T2.* " +
                                   "FROM auths_user_claims T1 JOIN auths_users T2 " +
                                   "ON T1.UserId = T2.Id " +
                                   "WHERE T2.ClaimType = @ClaimType " +
                                   "AND T2.ClaimValue = @ClaimValue; ";

            return
                 //IEnumerable<ApplicationUser> ApplicationUserClaims =
                 Task.Run(() => _connection.QueryAsync<ApplicationUser>(command, new
                 {
                     ClaimType = claim.Type,
                     ClaimValue = claim.Value,
                 }), cancellationToken)
                .ContinueWith(
                    o =>
                    {
                        IList<ApplicationUser> users = o.Result.Distinct().ToList();
                        return users;
                    });
            //.Result;

            //return Task.FromResult<IList<Claim>>(
            //    ApplicationUserClaims.Select(e => new Claim(
            //        e.ClaimType, e.ClaimValue)).ToList());
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
        // ~UsersClaimsTable() {
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
