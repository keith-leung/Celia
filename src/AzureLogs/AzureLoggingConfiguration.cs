using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace SharpCC.UtilityFramework.AzureLogs
{
    public class AzureLoggingConfiguration
    {
        public const string AZURE_LOGGING_CONFIG_NODES = "azureLoggings";

        public const string LOGGER_NAME = "AzureLoggerName";

        public const string CONNECTION_STRING_NODE_KEY = "key";

        public const string RUNTIME = "Runtime";

        public const string RUNTIME_DEBUG = "Debug";
        public const string RUNTIME_RELEASE = "Release";
        public const string RUNTIME_FORCE = "Force";

        public const string CONNECTION_STRING = "value";

        internal List<AzureLoggingConnectionString> ConnectionStrings
        {
            get { return this.m_connectionStrings; }
        }

        private List<AzureLoggingConnectionString> m_connectionStrings
            = new List<AzureLoggingConnectionString>();

        private AzureLoggingConnectionString m_defaultConnectionString = null;

        public bool NeedAzureLogging
        {
            get;
            internal set;
        }

        public string AzureLoggingStorageAccountConnection
        {
            get;
            set;
        }

        public string AzureLoggerName
        {
            get;
            set;
        }

        public CloudStorageAccount AzureLoggingStorageAccount { get; internal set; }
        public CloudTableClient AzureTableClient { get; internal set; }

        public void Build()
        {
            if (m_connectionStrings.Any(
                m => m.Runtime == ConfigSectionRuntimeEnum.FORCE))
            {//如果是强制使用，那么就使用这个节点
                m_defaultConnectionString = m_connectionStrings.First(
                    m => m.Runtime == ConfigSectionRuntimeEnum.FORCE);
            }
            else
            {
#if DEBUG
                if (m_connectionStrings.Any(
                    m => m.Runtime == ConfigSectionRuntimeEnum.DEBUG))
                {
                    m_defaultConnectionString = m_connectionStrings.First(
                        m => m.Runtime == ConfigSectionRuntimeEnum.DEBUG);
                }
#else
                if (m_connectionStrings.Any(
                    m => m.Runtime == ConfigSectionRuntimeEnum.RELEASE))
                {
                    m_defaultConnectionString = m_connectionStrings.First(
                        m => m.Runtime == ConfigSectionRuntimeEnum.RELEASE);
                } 
#endif
                if (m_defaultConnectionString == null //如果ConnectionString仍然为空
                    && m_connectionStrings.Any(
                   m => string.IsNullOrEmpty(m.Key)))
                {
                    m_defaultConnectionString = m_connectionStrings.First(
                        m => string.IsNullOrEmpty(m.Key));
                }

                if (m_defaultConnectionString == null //如果ConnectionString仍然为空
                    && m_connectionStrings.Any(
                   m => string.IsNullOrEmpty(m.Key)))
                {//随便取一个
                    m_defaultConnectionString = m_connectionStrings.FirstOrDefault();
                }
            }
            if (this.m_defaultConnectionString != null &&
                !string.IsNullOrWhiteSpace(this.m_defaultConnectionString.AzureStorageAccountConnection))
            {
                this.NeedAzureLogging = true;

                this.AzureLoggingStorageAccount = CloudStorageAccount.Parse(
                    this.m_defaultConnectionString.AzureStorageAccountConnection);
                this.AzureTableClient = AzureLoggingStorageAccount.CreateCloudTableClient();
            }

            if (this.m_defaultConnectionString != null &&
                !string.IsNullOrWhiteSpace(this.m_defaultConnectionString.AzureStorageAccountConnection))
            {
                this.AzureLoggerName = this.m_defaultConnectionString.AzureLoggerName;
            }
            else
            {
                this.AzureLoggerName = "DefaultLogger";
            }
        }
    }
}
