using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Celia.io.Core.Auths.Abstractions
{
    public interface IServiceAppRepository
    {
        Task<ServiceApp> GetByAppIdAppSecretAsync(string appId, string appSecret);
        Task UpdateAccessFailedCountAsync(string appId, string appSecret, int count);
        Task UpdateLockoutEnabled(string appId, string appSecret, bool enabled);
        Task UpdateLockoutEnd(string appId, string appSecret, DateTimeOffset? lockoutEnd);
    }
}
