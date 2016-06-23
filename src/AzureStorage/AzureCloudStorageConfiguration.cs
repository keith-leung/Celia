using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCC.UtilityFramework.AzureStorage
{
    public class AzureCloudStorageConfiguration
    {
        public const string CONNECTION_STRING = "StorageAccountConnectionString";

        public const string FILE_NODENAME = "file";
        public const string QUEUE_NODENAME = "queue";
        public const string TABLE_NODENAME = "table";
        public const string BLOB_NODENAME = "blob";

        public string AzureBlobAccountConnection { get; internal set; }
        public string AzureFileAccountConnection { get; internal set; }
        public string AzureQueueAccountConnection { get; internal set; }
        public string AzureStorageAccountConnection { get; internal set; }
        public string AzureTableAccountConnection { get; internal set; }

        internal void Build()
        {
            if (string.IsNullOrWhiteSpace(this.AzureBlobAccountConnection))
            {
                this.AzureBlobAccountConnection = this.AzureStorageAccountConnection;
            }
            if (string.IsNullOrWhiteSpace(this.AzureFileAccountConnection))
            {
                this.AzureFileAccountConnection = this.AzureStorageAccountConnection;
            }
            if (string.IsNullOrWhiteSpace(this.AzureQueueAccountConnection))
            {
                this.AzureQueueAccountConnection = this.AzureStorageAccountConnection;
            }
            if (string.IsNullOrWhiteSpace(this.AzureTableAccountConnection))
            {
                this.AzureTableAccountConnection = this.AzureStorageAccountConnection;
            }
        }
    }
}
