using Celia.io.Core.StaticObjects.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Celia.io.Core.StaticObjects.Services
{
    public interface IServiceAppService
    {
        bool IsValid(string appId, string appSecret);

        ServiceApp GetServiceAppByAppIdAppSecret(string appId, string appSecret);

        bool IsServiceAppLocked(string appId);

        void SetServiceAppLocked(string appId, DateTime lockUntilUtc);

        void UnlockServiceApp(string appId);
    }
}
