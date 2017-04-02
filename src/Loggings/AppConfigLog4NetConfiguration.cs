using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCC.UtilityFramework.Loggings
{
    public class AppConfigLog4NetConfiguration : ILog4NetConfiguration
    {
        public string LoggerName
        {
            get;
            internal set;
        }

        public AppConfigLog4NetConfiguration(string loggerName)
        {
            this.LoggerName = loggerName;
        }

        static AppConfigLog4NetConfiguration()
        {
            log4net.Config.XmlConfigurator.Configure();
        }
    }
}
