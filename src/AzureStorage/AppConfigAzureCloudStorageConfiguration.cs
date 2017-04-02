using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCC.UtilityFramework.AzureStorage
{
    public class AppConfigAzureCloudStorageConfiguration : IAzureCloudStorageConfiguration
    {
        public AppConfigAzureCloudStorageConfiguration()
        {

        }

        public const string AZURE_STORAGE_CONFIG_NODES = "azureStorages";
        public const string AZURE_STORAGE_NODE_NAME = "add";
        public const string CONNECTION_STRING_NODE_KEY = "key";
        public const string RUNTIME = "Runtime";

        public const string RUNTIME_DEBUG = "Debug";
        public const string RUNTIME_RELEASE = "Release";
        public const string RUNTIME_FORCE = "Force";

        public const string CONNECTION_STRING = "value";

        public const string FILE_NODENAME = "file";
        public const string QUEUE_NODENAME = "queue";
        public const string TABLE_NODENAME = "table";
        public const string BLOB_NODENAME = "blob";

        internal List<AzureStorageConnectionString> ConnectionStrings
        {
            get { return this.m_connectionStrings; }
        }

        private List<AzureStorageConnectionString> m_connectionStrings
            = new List<AzureStorageConnectionString>();

        private AzureStorageConnectionString m_defaultConnectionString = null;

        public string Key
        {
            get
            {
                if (m_defaultConnectionString != null)
                    return m_defaultConnectionString.Key;
                return string.Empty;
            }
        }

        public ConfigSectionRuntimeEnum Runtime
        {
            get
            {
                if (m_defaultConnectionString != null)
                    return m_defaultConnectionString.Runtime;
                return ConfigSectionRuntimeEnum.FORCE;
            }
        }

        public string AzureBlobAccountConnection
        {
            get
            {
                if (m_defaultConnectionString != null)
                    return m_defaultConnectionString.AzureBlobAccountConnection;
                return string.Empty;
            }
            set
            {
                if (m_defaultConnectionString != null)
                    m_defaultConnectionString.AzureBlobAccountConnection = value;
            }
        }

        public string AzureFileAccountConnection
        {
            get
            {
                if (m_defaultConnectionString != null)
                    return m_defaultConnectionString.AzureFileAccountConnection;
                return string.Empty;
            }
            set
            {
                if (m_defaultConnectionString != null)
                    m_defaultConnectionString.AzureFileAccountConnection = value;
            }
        }

        public string AzureQueueAccountConnection
        {
            get
            {
                if (m_defaultConnectionString != null)
                    return m_defaultConnectionString.AzureQueueAccountConnection;
                return string.Empty;
            }
            set
            {
                if (m_defaultConnectionString != null)
                    m_defaultConnectionString.AzureQueueAccountConnection = value;
            }
        }

        public string AzureStorageAccountConnection
        {
            get
            {
                if (m_defaultConnectionString != null)
                    return m_defaultConnectionString.AzureStorageAccountConnection;
                return string.Empty;
            }
            set
            {
                if (m_defaultConnectionString != null)
                    m_defaultConnectionString.AzureStorageAccountConnection = value;
            }
        }

        public string AzureTableAccountConnection
        {
            get
            {
                if (m_defaultConnectionString != null)
                    return m_defaultConnectionString.AzureTableAccountConnection;
                return string.Empty;
            }
            set
            {
                if (m_defaultConnectionString != null)
                    m_defaultConnectionString.AzureTableAccountConnection = value;
            }
        } 

        internal void Build()
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
        }
    }
}
