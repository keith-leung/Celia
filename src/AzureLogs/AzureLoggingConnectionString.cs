using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCC.UtilityFramework.AzureLogs
{
    internal class AzureLoggingConnectionString
    {
        public ConfigSectionRuntimeEnum Runtime
        {
            get;
            internal set;
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
