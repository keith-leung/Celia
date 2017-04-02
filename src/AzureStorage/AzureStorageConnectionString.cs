using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCC.UtilityFramework.AzureStorage
{
    public class AzureStorageConnectionString
    {
        public string Key { get; set; }
        public ConfigSectionRuntimeEnum Runtime { get; set; }
        public string AzureBlobAccountConnection { get; set; }
        public string AzureFileAccountConnection { get; set; }
        public string AzureQueueAccountConnection { get; set; }
        public string AzureStorageAccountConnection { get; set; }
        public string AzureTableAccountConnection { get; set; }

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

        /// <summary>
        /// ToString, Debugging use
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[{0}]|[{1}] ConnectionString={2}",
                this.Key, this.Runtime.ToString(), this.AzureStorageAccountConnection);
        }
    }
}
