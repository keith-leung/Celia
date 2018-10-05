using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Celia.io.Core.Utils
{
    public class DisconfService
    {
        private readonly IConfiguration _configuration;

        private readonly IDictionary<string, object> _customConfigs = new Dictionary<string, object>();

        public DisconfService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration
        {
            get; private set;
        }

        public IDictionary<string, object> CustomConfigs
        {
            get { return _customConfigs; }
        }
    }
}
