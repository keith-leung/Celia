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
    public class AzureCloudStorageContext : IAzureCloudStorageContext
    {
        public AzureCloudStorageContext(IAzureCloudStorageConfiguration configuration)
        {
            m_configuration = configuration;

            this.Build();
        }

        private IAzureCloudStorageConfiguration m_configuration = null;

        /*
        public AzureCloudStorageContext() :
            this(m_azureConfiguration.AzureStorageAccountConnection,
                m_azureConfiguration.AzureBlobAccountConnection,
                m_azureConfiguration.AzureFileAccountConnection,
                m_azureConfiguration.AzureQueueAccountConnection,
                m_azureConfiguration.AzureTableAccountConnection)
        {
            //m_azureStorageAccountConnection = m_azureConfiguration.AzureStorageAccountConnection;
            //m_azureBlobAccountConnection = m_azureConfiguration.AzureBlobAccountConnection;
            //m_azureFileAccountConnection = m_azureConfiguration.AzureFileAccountConnection;
            //m_azureQueueAccountConnection = m_azureConfiguration.AzureQueueAccountConnection;
            //m_azureTableAccountConnection = m_azureConfiguration.AzureTableAccountConnection;

            //this.Build();
        }

        public AzureCloudStorageContext(string connectionString) :
            this(connectionString, connectionString, connectionString,
                connectionString, connectionString)
        {
            //m_azureStorageAccountConnection = m_azureConfiguration.AzureStorageAccountConnection;
            //m_azureBlobAccountConnection = m_azureConfiguration.AzureBlobAccountConnection;
            //m_azureFileAccountConnection = m_azureConfiguration.AzureFileAccountConnection;
            //m_azureQueueAccountConnection = m_azureConfiguration.AzureQueueAccountConnection;
            //m_azureTableAccountConnection = m_azureConfiguration.AzureTableAccountConnection;

            //this.Build();
        }

        public AzureCloudStorageContext(string connectionString,
            string blobConnectionString, string fileConnectionString,
            string queueConnectionString, string tableConnectionString)
        {
            m_azureStorageAccountConnection = connectionString;
            m_azureBlobAccountConnection = blobConnectionString;
            m_azureFileAccountConnection = fileConnectionString;
            m_azureQueueAccountConnection = queueConnectionString;
            m_azureTableAccountConnection = tableConnectionString;

            this.Build();
        }

        static AzureCloudStorageContext()
        {
            try
            {
                m_azureConfiguration = System.Configuration.ConfigurationManager.GetSection("azureStorages") as AppConfigAzureCloudStorageConfiguration;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }
        */

        //private static AppConfigAzureCloudStorageConfiguration m_azureConfiguration = null;

        //private string m_azureStorageAccountConnection = string.Empty;
        //private string m_azureBlobAccountConnection = string.Empty;
        //private string m_azureFileAccountConnection = string.Empty;
        //private string m_azureQueueAccountConnection = string.Empty;
        //private string m_azureTableAccountConnection = string.Empty;

        #region init
        private void Build()
        {
            this.BuildStorage();
            this.BuildBlob();
            this.BuildFile();
            this.BuildQueue();
            this.BuildTable();
        }

        private void BuildStorage()
        {
            if (m_azureStorageAccount == null)
            {
                m_azureStorageAccount = CloudStorageAccount.Parse(
                    this.AzureStorageAccountConnection);
                //m_azureStorageAccountConnection);
            }
        }

        private void BuildBlob()
        {
            this.BuildStorage();

            if (!this.AzureStorageAccountConnection.Equals(
                this.AzureBlobAccountConnection))
            {
                m_azureBlobStorageAccount = CloudStorageAccount.Parse(
                    this.AzureBlobAccountConnection);
                return;
            }
            else
                this.m_azureBlobStorageAccount = this.AzureStorageAccount;
            //return this.AzureStorageAccount;
        }

        private void BuildFile()
        {
            this.BuildStorage();

            if (!this.AzureStorageAccountConnection.Equals(
                this.AzureFileAccountConnection))
            {
                m_azureFileStorageAccount = CloudStorageAccount.Parse(
                    this.AzureFileAccountConnection);
                return;
            }
            else
                this.m_azureFileStorageAccount = this.AzureStorageAccount;
            //return this.AzureStorageAccount;
        }

        private void BuildQueue()
        {
            this.BuildStorage();

            if (!this.AzureStorageAccountConnection.Equals(
                this.AzureQueueAccountConnection))
            {
                m_azureQueueStorageAccount = CloudStorageAccount.Parse(
                    this.AzureQueueAccountConnection);
                return;
            }

            this.m_azureQueueStorageAccount = this.AzureStorageAccount;
            //return this.AzureStorageAccount;
        }

        private void BuildTable()
        {
            this.BuildStorage();

            if (!this.AzureStorageAccountConnection.Equals(
                this.AzureTableAccountConnection))
            {
                m_azureTableStorageAccount = CloudStorageAccount.Parse(
                    this.AzureTableAccountConnection);
                return;
            }
            else
                this.m_azureTableStorageAccount = this.AzureStorageAccount;
            //return this.AzureStorageAccount;
        }
        #endregion init

        public string AzureBlobAccountConnection
        {
            get { return this.m_configuration.AzureBlobAccountConnection; }
            set
            {
                this.m_configuration.AzureBlobAccountConnection = value;
                this.BuildBlob();
            }
        }

        public string AzureFileAccountConnection
        {
            get { return this.m_configuration.AzureFileAccountConnection; }
            set
            {
                this.m_configuration.AzureFileAccountConnection = value;
                this.BuildFile();
            }
        }

        public string AzureQueueAccountConnection
        {
            get { return this.m_configuration.AzureQueueAccountConnection; }
            set
            {
                this.m_configuration.AzureQueueAccountConnection = value;
                this.BuildQueue();
            }
        }

        public string AzureTableAccountConnection
        {
            get { return this.m_configuration.AzureTableAccountConnection; }
            set
            {
                this.m_configuration.AzureTableAccountConnection = value;
                this.BuildTable();
            }
        }

        public string AzureStorageAccountConnection
        {
            get { return this.m_configuration.AzureStorageAccountConnection; }
            set
            {
                this.m_configuration.AzureStorageAccountConnection = value;
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
