using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SharpCC.UtilityFramework.AzureStorage
{
    public class AzureStorageConfigurationSectionHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            AzureCloudStorageConfiguration config = new AzureCloudStorageConfiguration();

            if (section.Attributes["key"] != null && section.Attributes["value"] != null)
            {
                string rootKey = section.Attributes["key"].Value;
                if (rootKey == AzureCloudStorageConfiguration.CONNECTION_STRING)
                {
                    config.AzureStorageAccountConnection = section.Attributes["value"].Value;
                }
            }

            string key = string.Empty;

            foreach (XmlNode childNode in section.ChildNodes)
            {
                try
                {
                    if (childNode.Attributes["key"] != null && childNode.Attributes["value"] != null)
                    {
                        key = childNode.Attributes["key"].Value;
                        if (key == AzureCloudStorageConfiguration.CONNECTION_STRING)
                        {
                            if (childNode.Name.Equals(AzureCloudStorageConfiguration.BLOB_NODENAME, StringComparison.InvariantCultureIgnoreCase))
                                config.AzureBlobAccountConnection = section.Attributes["value"].Value;
                            if (childNode.Name.Equals(AzureCloudStorageConfiguration.QUEUE_NODENAME, StringComparison.InvariantCultureIgnoreCase))
                                config.AzureQueueAccountConnection = section.Attributes["value"].Value;
                            if (childNode.Name.Equals(AzureCloudStorageConfiguration.TABLE_NODENAME, StringComparison.InvariantCultureIgnoreCase))
                                config.AzureTableAccountConnection = section.Attributes["value"].Value;
                            if (childNode.Name.Equals(AzureCloudStorageConfiguration.FILE_NODENAME, StringComparison.InvariantCultureIgnoreCase))
                                config.AzureFileAccountConnection = section.Attributes["value"].Value;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    System.Diagnostics.Trace.TraceError(ex.Message);
                }
            }

            config.Build();

            return config;
        }
    }
}
