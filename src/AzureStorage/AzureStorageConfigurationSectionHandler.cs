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
            AppConfigAzureCloudStorageConfiguration config = new AppConfigAzureCloudStorageConfiguration();

            if (section.Name.Equals(AppConfigAzureCloudStorageConfiguration.AZURE_STORAGE_CONFIG_NODES,
               StringComparison.InvariantCultureIgnoreCase))
            {
                foreach (XmlNode childNode in section.ChildNodes)
                {
                    try
                    {
                        string key = string.Empty;
                        string runtime = string.Empty;
                        string value = string.Empty;

                        AzureStorageConnectionString connStr = new AzureStorageConnectionString();

                        if (childNode.Attributes != null && childNode.Attributes.Count > 0 &&
                            childNode.Attributes[AppConfigAzureCloudStorageConfiguration.CONNECTION_STRING] != null)
                        {
                            value = childNode.Attributes[AppConfigAzureCloudStorageConfiguration.CONNECTION_STRING].Value;

                            if (childNode.Attributes[AppConfigAzureCloudStorageConfiguration.CONNECTION_STRING_NODE_KEY] != null)
                            {
                                key = childNode.Attributes[AppConfigAzureCloudStorageConfiguration.CONNECTION_STRING_NODE_KEY].Value;
                            }

                            ConfigSectionRuntimeEnum rt = ConfigSectionRuntimeEnum.RELEASE;
                            if (childNode.Attributes[AppConfigAzureCloudStorageConfiguration.RUNTIME] != null)
                            {
                                runtime = childNode.Attributes[AppConfigAzureCloudStorageConfiguration.RUNTIME].Value;
                                if (!string.IsNullOrEmpty(runtime) && runtime.Equals(
                                    AppConfigAzureCloudStorageConfiguration.RUNTIME_DEBUG, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    rt = ConfigSectionRuntimeEnum.DEBUG;
                                }
                                else if (!string.IsNullOrEmpty(runtime) && runtime.Equals(
                                   AppConfigAzureCloudStorageConfiguration.RUNTIME_FORCE, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    rt = ConfigSectionRuntimeEnum.FORCE;
                                }
                            }

                            connStr.Runtime = rt;
                            connStr.Key = key;
                            connStr.AzureStorageAccountConnection = value;

                            if (childNode.ChildNodes != null)
                            {
                                foreach (XmlNode child in childNode.ChildNodes)
                                {
                                    if (child.Name.Equals(AppConfigAzureCloudStorageConfiguration.BLOB_NODENAME,
                                        StringComparison.InvariantCultureIgnoreCase)
                                        && section.Attributes["value"] != null)
                                        connStr.AzureBlobAccountConnection = section.Attributes["value"].Value;
                                    if (child.Name.Equals(AppConfigAzureCloudStorageConfiguration.QUEUE_NODENAME,
                                        StringComparison.InvariantCultureIgnoreCase)
                                        && section.Attributes["value"] != null)
                                        connStr.AzureQueueAccountConnection = section.Attributes["value"].Value;
                                    if (child.Name.Equals(AppConfigAzureCloudStorageConfiguration.TABLE_NODENAME,
                                        StringComparison.InvariantCultureIgnoreCase)
                                        && section.Attributes["value"] != null)
                                        connStr.AzureTableAccountConnection = section.Attributes["value"].Value;
                                    if (child.Name.Equals(AppConfigAzureCloudStorageConfiguration.FILE_NODENAME,
                                        StringComparison.InvariantCultureIgnoreCase)
                                        && section.Attributes["value"] != null)
                                        connStr.AzureFileAccountConnection = section.Attributes["value"].Value;
                                }
                            }

                            connStr.Build();
                            config.ConnectionStrings.Add(connStr);

                            #region old
                            //if (section.Attributes["key"] != null && section.Attributes["value"] != null)
                            //{
                            //    string rootKey = section.Attributes["key"].Value;
                            //    if (rootKey == AzureCloudStorageConfiguration.CONNECTION_STRING)
                            //    {
                            //        config.AzureStorageAccountConnection = section.Attributes["value"].Value;
                            //    }
                            //}

                            //if (childNode.Attributes["key"] != null && childNode.Attributes["value"] != null)
                            //{
                            //    key = childNode.Attributes["key"].Value;
                            //    if (key == AzureCloudStorageConfiguration.CONNECTION_STRING)
                            //    {
                            //        if (childNode.Name.Equals(AzureCloudStorageConfiguration.BLOB_NODENAME, StringComparison.InvariantCultureIgnoreCase))
                            //            config.AzureBlobAccountConnection = section.Attributes["value"].Value;
                            //        if (childNode.Name.Equals(AzureCloudStorageConfiguration.QUEUE_NODENAME, StringComparison.InvariantCultureIgnoreCase))
                            //            config.AzureQueueAccountConnection = section.Attributes["value"].Value;
                            //        if (childNode.Name.Equals(AzureCloudStorageConfiguration.TABLE_NODENAME, StringComparison.InvariantCultureIgnoreCase))
                            //            config.AzureTableAccountConnection = section.Attributes["value"].Value;
                            //        if (childNode.Name.Equals(AzureCloudStorageConfiguration.FILE_NODENAME, StringComparison.InvariantCultureIgnoreCase))
                            //            config.AzureFileAccountConnection = section.Attributes["value"].Value;
                            //    }
                            //}
                            #endregion
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        System.Diagnostics.Trace.TraceError(ex.Message);
                    }
                }

                config.Build();
            }

            return config;
        }
    }
}
