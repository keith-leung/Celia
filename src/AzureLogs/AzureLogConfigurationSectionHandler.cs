using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SharpCC.UtilityFramework.AzureLogs
{
    public class AzureLogConfigurationSectionHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            AzureLoggingConfiguration config = new AzureLoggingConfiguration();

            if (section.Name.Equals(AzureLoggingConfiguration.AZURE_LOGGING_CONFIG_NODES,
                StringComparison.InvariantCultureIgnoreCase))
            {
                foreach (XmlNode childNode in section.ChildNodes)
                {
                    try
                    {
                        string key = string.Empty;
                        string runtime = string.Empty;
                        string value = string.Empty;

                        AzureLoggingConnectionString connStr = new AzureLoggingConnectionString();

                        if (childNode.Attributes != null && childNode.Attributes.Count > 0 &&
                            childNode.Attributes[AzureLoggingConfiguration.CONNECTION_STRING] != null)
                        {
                            value = childNode.Attributes[AzureLoggingConfiguration.CONNECTION_STRING].Value;

                            if (childNode.Attributes[AzureLoggingConfiguration.CONNECTION_STRING_NODE_KEY] != null)
                            {
                                key = childNode.Attributes[AzureLoggingConfiguration.CONNECTION_STRING_NODE_KEY].Value;
                            }

                            ConfigSectionRuntimeEnum rt = ConfigSectionRuntimeEnum.RELEASE;
                            if (childNode.Attributes[AzureLoggingConfiguration.RUNTIME] != null)
                            {
                                runtime = childNode.Attributes[AzureLoggingConfiguration.RUNTIME].Value;
                                if (!string.IsNullOrEmpty(runtime) && runtime.Equals(
                                    AzureLoggingConfiguration.RUNTIME_DEBUG, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    rt = ConfigSectionRuntimeEnum.DEBUG;
                                }
                                else if (!string.IsNullOrEmpty(runtime) && runtime.Equals(
                                   AzureLoggingConfiguration.RUNTIME_FORCE, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    rt = ConfigSectionRuntimeEnum.FORCE;
                                }
                            }

                            connStr.Runtime = rt;
                            connStr.Key = key;
                            connStr.AzureStorageAccountConnection = value;

                            if (childNode.Attributes[AzureLoggingConfiguration.LOGGER_NAME] != null)
                            {
                                connStr.AzureLoggerName = childNode.Attributes[AzureLoggingConfiguration.LOGGER_NAME].Value;
                            }

                            config.ConnectionStrings.Add(connStr);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        System.Diagnostics.Trace.TraceError(ex.Message);
                    }
                }
            }

            config.Build();

            return config;
        }
    }
}
