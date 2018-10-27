using System;
using System.Collections.Generic;
using System.Text;
using Celia.io.Core.StaticObjects.Abstractions;
using Microsoft.Extensions.Logging;

namespace Celia.io.Core.StaticObjects.Services.Impl
{
    public class ServiceAppService : IServiceAppService
    { 
        private readonly ILogger<ServiceAppService> _logger;
        private readonly IStaticObjectsRepository _repository;
        private readonly IServiceProvider _serviceProvider;

        public ServiceAppService(ILogger<ServiceAppService> logger, IStaticObjectsRepository repository,
            IServiceProvider serviceProvider)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this._serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public ServiceApp GetServiceAppByAppIdAppSecret(string appId, string appSecret)
        {
            return new ServiceApp()
            {
                AppId = appId,
                AppSecret = appSecret,
                CTIME = DateTime.Now,
                Description = "TODO:"
            };
        }

        public bool IsServiceAppLocked(string appId)
        {//需要对接账号中心
            return false;
        }

        public bool IsValid(string appId, string appSecret)
        {//需要对接账号中心
            return true;
        }

        public void SetServiceAppLocked(string appId, DateTime lockUntilUtc)
        {
            //
        }

        public void UnlockServiceApp(string appId)
        {
            //
        }
    }
}
