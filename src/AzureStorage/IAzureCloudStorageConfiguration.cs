namespace SharpCC.UtilityFramework.AzureStorage
{
    public interface IAzureCloudStorageConfiguration
    {
        string AzureBlobAccountConnection { get; set; }
        string AzureFileAccountConnection { get; set; }
        string AzureQueueAccountConnection { get; set; }
        string AzureStorageAccountConnection { get; set; }
        string AzureTableAccountConnection { get; set; }
    }
}