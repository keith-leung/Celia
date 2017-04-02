using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SharpCC.UtilityFramework.Loggings.AzureLogs
{
    public class AzureLogConfigurationSectionHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            AppConfigAzureLoggingConfiguration config = new AppConfigAzureLoggingConfiguration();

            if (section.Name.Equals(AppConfigAzureLoggingConfiguration.AZURE_LOGGING_CONFIG_NODES,
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
                            childNode.Attributes[AppConfigAzureLoggingConfiguration.CONNECTION_STRING] != null)
                        {
                            value = childNode.Attributes[AppConfigAzureLoggingConfiguration.CONNECTION_STRING].Value;

                            if (childNode.Attributes[AppConfigAzureLoggingConfiguration.CONNECTION_STRING_NODE_KEY] != null)
                            {
                                key = childNode.Attributes[AppConfigAzureLoggingConfiguration.CONNECTION_STRING_NODE_KEY].Value;
                            }

                            ConfigSectionRuntimeEnum rt = ConfigSectionRuntimeEnum.RELEASE;
                            if (childNode.Attributes[AppConfigAzureLoggingConfiguration.RUNTIME] != null)
                            {
                                runtime = childNode.Attributes[AppConfigAzureLoggingConfiguration.RUNTIME].Value;
                                if (!string.IsNullOrEmpty(runtime) && runtime.Equals(
                                    AppConfigAzureLoggingConfiguration.RUNTIME_DEBUG, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    rt = ConfigSectionRuntimeEnum.DEBUG;
                                }
                                else if (!string.IsNullOrEmpty(runtime) && runtime.Equals(
                                   AppConfigAzureLoggingConfiguration.RUNTIME_FORCE, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    rt = ConfigSectionRuntimeEnum.FORCE;
                                }
                            }

                            connStr.Runtime = rt;
                            connStr.Key = key;
                            connStr.AzureStorageAccountConnection = value;

                            if (childNode.Attributes[AppConfigAzureLoggingConfiguration.LOGGER_NAME] != null)
                            {
                                connStr.AzureLoggerName = childNode.Attributes[AppConfigAzureLoggingConfiguration.LOGGER_NAME].Value;
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
