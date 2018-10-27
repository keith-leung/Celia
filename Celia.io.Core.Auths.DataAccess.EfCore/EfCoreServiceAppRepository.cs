using Celia.io.Core.Auths.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Celia.io.Core.Auths.DataAccess.EfCore
{
    public class EfCoreServiceAppRepository : IServiceAppRepository
    {
        private readonly ApplicationDbContext _context = null;
        private readonly ILogger logger = null;

        public EfCoreServiceAppRepository(ILogger<EfCoreRoleStore> logger, ApplicationDbContext context)
        {
            this._context = context ?? throw new ArgumentNullException(
                nameof(context));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private List<ServiceApp> _serviceApps = new List<ServiceApp>();

        /// <summary>
        /// 通过内存缓存ServiceApp实现快速查询
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        public Task<ServiceApp> GetByAppIdAppSecretAsync(string appId, string appSecret)
        {
            return Task.Run<ServiceApp>(() =>
            {
                var serviceApp = _serviceApps.Find(m => m.AppId.Equals(
                    appId, StringComparison.InvariantCultureIgnoreCase)
                    && m.AppSecret.Equals(appSecret, StringComparison.InvariantCultureIgnoreCase));
                if (serviceApp == null)
                {
                    serviceApp = _context.ServiceApps.FirstOrDefault(m => m.AppId.Equals(
                    appId, StringComparison.InvariantCultureIgnoreCase)
                    && m.AppSecret.Equals(appSecret, StringComparison.InvariantCultureIgnoreCase));

                    if (serviceApp != null)
                        _serviceApps.Add(serviceApp);
                }

                return serviceApp;
            });
        }

        public Task UpdateAccessFailedCountAsync(string appId, string appSecret, int count)
        {
            return Task.Run(() =>
            {
                var serviceApp = _context.ServiceApps.FirstOrDefault(m => m.AppId.Equals(
                    appId, StringComparison.InvariantCultureIgnoreCase)
                    && m.AppSecret.Equals(appSecret, StringComparison.InvariantCultureIgnoreCase));

                if (serviceApp != null)
                {
                    serviceApp.AccessFailedCount = count;
                    _context.SaveChanges();
                }

                var serviceApp2 = _serviceApps.Find(m => m.AppId.Equals(
                    appId, StringComparison.InvariantCultureIgnoreCase)
                    && m.AppSecret.Equals(appSecret, StringComparison.InvariantCultureIgnoreCase));
                if (serviceApp2 != null)
                {//被修改过，换出缓存
                    _serviceApps.Remove(serviceApp2);
                }
            });
        }

        public Task UpdateLockoutEnabled(string appId, string appSecret, bool enabled)
        {
            return Task.Run(() =>
            {
                var serviceApp = _context.ServiceApps.FirstOrDefault(m => m.AppId.Equals(
                    appId, StringComparison.InvariantCultureIgnoreCase)
                    && m.AppSecret.Equals(appSecret, StringComparison.InvariantCultureIgnoreCase));

                if (serviceApp != null)
                {
                    serviceApp.LockoutEnabled = enabled;
                    _context.SaveChanges();
                }

                var serviceApp2 = _serviceApps.Find(m => m.AppId.Equals(
                    appId, StringComparison.InvariantCultureIgnoreCase)
                    && m.AppSecret.Equals(appSecret, StringComparison.InvariantCultureIgnoreCase));
                if (serviceApp2 != null)
                {//被修改过，换出缓存
                    _serviceApps.Remove(serviceApp2);
                }
            });
        }

        public Task UpdateLockoutEnd(string appId, string appSecret, DateTimeOffset? lockoutEnd)
        {
            return Task.Run(() =>
            {
                var serviceApp = _context.ServiceApps.FirstOrDefault(m => m.AppId.Equals(
                    appId, StringComparison.InvariantCultureIgnoreCase)
                    && m.AppSecret.Equals(appSecret, StringComparison.InvariantCultureIgnoreCase));

                if (serviceApp != null)
                {
                    serviceApp.LockoutEnd = lockoutEnd;
                    _context.SaveChanges();
                }

                var serviceApp2 = _serviceApps.Find(m => m.AppId.Equals(
                    appId, StringComparison.InvariantCultureIgnoreCase)
                    && m.AppSecret.Equals(appSecret, StringComparison.InvariantCultureIgnoreCase));
                if (serviceApp2 != null)
                {//被修改过，换出缓存
                    _serviceApps.Remove(serviceApp2);
                }
            });
        }
    }
}
