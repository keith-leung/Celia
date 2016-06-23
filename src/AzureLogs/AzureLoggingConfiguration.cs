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
        private bool needAzureLogging = false;
        public const string LOGGER_NAME = "AzureLoggerName";
        public const string CONNECTION_STRING = "AzureLoggingStorageAccountConnection";

        public bool NeedAzureLogging
        {
            get { return needAzureLogging; }
            set { needAzureLogging = value; }
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
            string storageConnection = this.AzureLoggingStorageAccountConnection;
            if (!string.IsNullOrWhiteSpace(storageConnection))
            {
                this.needAzureLogging = true;

                this.AzureLoggingStorageAccount = CloudStorageAccount.Parse(storageConnection);
                this.AzureTableClient = AzureLoggingStorageAccount.CreateCloudTableClient();
            }

            if (string.IsNullOrWhiteSpace(this.AzureLoggerName))
            {
                this.AzureLoggerName = "DefaultLogger";
            }
        }
    }
}
