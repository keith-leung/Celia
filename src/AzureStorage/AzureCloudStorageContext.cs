using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.File;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCC.UtilityFramework.AzureStorage
{
    public class AzureCloudStorageContext
    {
        #region init
        static AzureCloudStorageContext()
        {
            try
            {
                m_azureConfiguration = System.Configuration.ConfigurationManager.GetSection("azureStorage") as AzureCloudStorageConfiguration;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        private static AzureCloudStorageConfiguration m_azureConfiguration = null;

        private AzureCloudStorageConfiguration m_config = null;

        internal AzureCloudStorageConfiguration Configuration
        {
            get
            {
                if (m_config != null)
                    return m_config;
                return m_azureConfiguration;
            }
        }

        public AzureCloudStorageContext()
        {
            m_config = new AzureCloudStorageConfiguration()
            {
                AzureStorageAccountConnection = m_azureConfiguration.AzureStorageAccountConnection,
                AzureBlobAccountConnection = m_azureConfiguration.AzureBlobAccountConnection,
                AzureFileAccountConnection = m_azureConfiguration.AzureFileAccountConnection,
                AzureQueueAccountConnection = m_azureConfiguration.AzureQueueAccountConnection,
                AzureTableAccountConnection = m_azureConfiguration.AzureTableAccountConnection,
            };
            m_config.Build();
        }

        public AzureCloudStorageContext(string connectionString)
        {
            m_config = new AzureCloudStorageConfiguration()
            {
                AzureStorageAccountConnection = connectionString
            };
            m_config.Build();
        }
        #endregion init

        #region storage account
        private CloudStorageAccount m_azureStorageAccount;

        public CloudStorageAccount AzureStorageAccount
        {
            get
            {
                if (m_azureStorageAccount == null)
                {
                    m_azureStorageAccount = CloudStorageAccount.Parse(
                        this.Configuration.AzureStorageAccountConnection);
                }
                return m_azureStorageAccount;
            }
        }

        private CloudStorageAccount m_azureTableStorageAccount;

        public CloudStorageAccount AzureTableStorageAccount
        {
            get
            {
                if (m_azureTableStorageAccount != null)
                    return m_azureTableStorageAccount;
                if (!this.Configuration.AzureStorageAccountConnection.Equals(
                    this.Configuration.AzureTableAccountConnection))
                {
                    m_azureTableStorageAccount = CloudStorageAccount.Parse(
                        this.Configuration.AzureTableAccountConnection);
                    return m_azureTableStorageAccount;
                }
                return this.AzureStorageAccount;
            }
        }

        private CloudStorageAccount m_azureFileStorageAccount;

        public CloudStorageAccount AzureFileStorageAccount
        {
            get
            {
                if (m_azureFileStorageAccount != null)
                    return m_azureFileStorageAccount;
                if (!this.Configuration.AzureStorageAccountConnection.Equals(
                    this.Configuration.AzureFileAccountConnection))
                {
                    m_azureFileStorageAccount = CloudStorageAccount.Parse(
                        this.Configuration.AzureFileAccountConnection);
                    return m_azureFileStorageAccount;
                }
                return this.AzureStorageAccount;
            }
        }

        private CloudStorageAccount m_azureBlobStorageAccount;

        public CloudStorageAccount AzureBlobStorageAccount
        {
            get
            {
                if (m_azureBlobStorageAccount != null)
                    return m_azureBlobStorageAccount;
                if (!this.Configuration.AzureStorageAccountConnection.Equals(
                    this.Configuration.AzureBlobAccountConnection))
                {
                    m_azureBlobStorageAccount = CloudStorageAccount.Parse(
                        this.Configuration.AzureBlobAccountConnection);
                    return m_azureBlobStorageAccount;
                }
                return this.AzureStorageAccount;
            }
        }

        private CloudStorageAccount m_azureQueueStorageAccount;

        public CloudStorageAccount AzureQueueStorageAccount
        {
            get
            {
                if (m_azureQueueStorageAccount != null)
                    return m_azureQueueStorageAccount;
                if (!this.Configuration.AzureStorageAccountConnection.Equals(
                    this.Configuration.AzureQueueAccountConnection))
                {
                    m_azureQueueStorageAccount = CloudStorageAccount.Parse(
                        this.Configuration.AzureQueueAccountConnection);
                    return m_azureQueueStorageAccount;
                }
                return this.AzureStorageAccount;
            }
        }
        #endregion storage account

        #region cloud client
        private CloudTableClient m_tableClient = null;

        public CloudTableClient AzureTableClient
        {
            get
            {
                if (m_tableClient == null)
                {
                    m_tableClient = this.AzureTableStorageAccount.CreateCloudTableClient();
                }
                return m_tableClient;
            }
        }

        private CloudQueueClient m_queueClient = null;

        public CloudQueueClient AzureQueueClient
        {
            get
            {
                if (m_queueClient == null)
                {
                    m_queueClient = this.AzureQueueStorageAccount.CreateCloudQueueClient();
                }
                return m_queueClient;
            }
        }

        private CloudFileClient m_fileClient = null;

        public CloudFileClient AzureFileClient
        {
            get
            {
                if (m_fileClient == null)
                {
                    m_fileClient = this.AzureFileStorageAccount.CreateCloudFileClient();
                }
                return m_fileClient;
            }
        }

        private CloudBlobClient m_blobClient = null;

        public CloudBlobClient AzureBlobClient
        {
            get
            {
                if (m_blobClient == null)
                {
                    m_blobClient = this.AzureBlobStorageAccount.CreateCloudBlobClient();
                }
                return m_blobClient;
            }
        }
        #endregion cloud client
    }
}
