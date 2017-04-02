using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.File;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;

namespace SharpCC.UtilityFramework.AzureStorage
{
    public interface IAzureCloudStorageContext
    {
        string AzureBlobAccountConnection { get; set; }
        CloudBlobClient AzureBlobClient { get; }
        CloudStorageAccount AzureBlobStorageAccount { get; }
        string AzureFileAccountConnection { get; set; }
        CloudFileClient AzureFileClient { get; }
        CloudStorageAccount AzureFileStorageAccount { get; }
        string AzureQueueAccountConnection { get; set; }
        CloudQueueClient AzureQueueClient { get; }
        CloudStorageAccount AzureQueueStorageAccount { get; }
        CloudStorageAccount AzureStorageAccount { get; }
        string AzureStorageAccountConnection { get; set; }
        string AzureTableAccountConnection { get; set; }
        CloudTableClient AzureTableClient { get; }
        CloudStorageAccount AzureTableStorageAccount { get; }
    }
}