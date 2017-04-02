using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCC.UtilityFramework.Loggings.AzureLogs
{
    public class AzureLoggingConnectionString
    {
        public ConfigSectionRuntimeEnum Runtime
        {
            get;
            set;
        }

        /// <summary>
        /// 标志连接字符串的KEY
        /// </summary>
        public string Key { get; internal set; }

        public string AzureStorageAccountConnection { get; internal set; }
        public string AzureLoggerName { get; internal set; }

        /// <summary>
        /// ToString, Debugging use
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[{0}]|[{1}] ConnectionString={2}; @{5}",
                this.Key, this.Runtime.ToString(), this.AzureStorageAccountConnection, this.AzureLoggerName);
        }
    }
}
