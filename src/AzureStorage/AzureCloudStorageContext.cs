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
                if (m_config == null)
                {
                    m_config = new AzureCloudStorageConfiguration();
                }
                return m_config;
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

            this.Build();
        }

        private void Build()
        {
            this.BuildStorage();
            this.BuildBlob();
            this.BuildQueue();
            this.BuildTable();
        }

        private void BuildStorage()
        {
            if (m_azureTableStorageAccount == null)
            {
                m_azureStorageAccount = CloudStorageAccount.Parse(
                    this.Configuration.AzureStorageAccountConnection);
            }
        }

        private void BuildTable()
        {
            this.BuildStorage();

            if (!this.Configuration.AzureStorageAccountConnection.Equals(
                this.Configuration.AzureTableAccountConnection))
            {
                m_azureTableStorageAccount = CloudStorageAccount.Parse(
                    this.Configuration.AzureTableAccountConnection);
                return;
            }

            this.m_azureTableStorageAccount = this.AzureStorageAccount;
            //return this.AzureStorageAccount;
        }

        private void BuildFile()
        {
            this.BuildStorage();

            if (!this.Configuration.AzureStorageAccountConnection.Equals(
                this.Configuration.AzureFileAccountConnection))
            {
                m_azureFileStorageAccount = CloudStorageAccount.Parse(
                    this.Configuration.AzureFileAccountConnection);
                return;
            }

            this.m_azureFileStorageAccount = this.AzureStorageAccount;
            //return this.AzureStorageAccount;
        }

        private void BuildQueue()
        {
            this.BuildStorage();

            if (!this.Configuration.AzureStorageAccountConnection.Equals(
                this.Configuration.AzureQueueAccountConnection))
            {
                m_azureQueueStorageAccount = CloudStorageAccount.Parse(
                    this.Configuration.AzureQueueAccountConnection);
                return;
            }

            this.m_azureQueueStorageAccount = this.AzureStorageAccount;
            //return this.AzureStorageAccount;
        }

        private void BuildBlob()
        {
            this.BuildStorage();

            if (!this.Configuration.AzureStorageAccountConnection.Equals(
                this.Configuration.AzureBlobAccountConnection))
            {
                m_azureBlobStorageAccount = CloudStorageAccount.Parse(
                    this.Configuration.AzureBlobAccountConnection);
                return;
            }

            this.m_azureBlobStorageAccount = this.AzureStorageAccount;
            //return this.AzureStorageAccount;
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

        public string AzureBlobAccountConnection
        {
            get { return this.m_config.AzureBlobAccountConnection; }
            set
            {
                this.m_config.AzureBlobAccountConnection = value;
                this.BuildBlob();
            }
        }

        public string AzureFileAccountConnection
        {
            get { return this.m_config.AzureFileAccountConnection; }
            set
            {
                this.m_config.AzureFileAccountConnection = value;
                this.BuildFile();
            }
        }

        public string AzureQueueAccountConnection
        {
            get { return this.m_config.AzureQueueAccountConnection; }
            set
            {
                this.m_config.AzureQueueAccountConnection = value;
                this.BuildQueue();
            }
        }

        public string AzureTableAccountConnection
        {
            get { return this.m_config.AzureTableAccountConnection; }
            set
            {
                this.m_config.AzureTableAccountConnection = value;
                this.BuildTable();
            }
        }

        public string AzureStorageAccountConnection
        {
            get { return this.m_config.AzureStorageAccountConnection; }
            set
            {
                this.m_config.AzureStorageAccountConnection = value;
                this.Build();
            }
        }

        #region storage account
        private CloudStorageAccount m_azureStorageAccount;

        public CloudStorageAccount AzureStorageAccount
        {
            get
            {
                if (m_azureStorageAccount == null)
                {
                    this.BuildStorage();
                }
                return m_azureStorageAccount;
            }
        }

        private CloudStorageAccount m_azureTableStorageAccount;

        public CloudStorageAccount AzureTableStorageAccount
        {
            get
            {
                if (this.m_azureTableStorageAccount == null)
                    this.BuildTable();

                return this.m_azureTableStorageAccount;
            }
        }

        private CloudStorageAccount m_azureFileStorageAccount;

        public CloudStorageAccount AzureFileStorageAccount
        {
            get
            {
                if (this.m_azureFileStorageAccount == null)
                    this.BuildFile();

                return this.m_azureFileStorageAccount;
            }
        }

        private CloudStorageAccount m_azureBlobStorageAccount;

        public CloudStorageAccount AzureBlobStorageAccount
        {
            get
            {
                if (this.m_azureBlobStorageAccount == null)
                    this.BuildBlob();

                return this.m_azureBlobStorageAccount;
            }
        }

        private CloudStorageAccount m_azureQueueStorageAccount;

        public CloudStorageAccount AzureQueueStorageAccount
        {
            get
            {
                if (this.m_azureQueueStorageAccount == null)
                    this.BuildQueue();

                return this.m_azureQueueStorageAccount;
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
