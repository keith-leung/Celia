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
            Dictionary<string, string> configValues = new Dictionary<string, string>();
            string key = string.Empty;

            foreach (XmlNode childNode in section.ChildNodes)
            {
                try
                {
                    if (childNode.Attributes["key"] != null && childNode.Attributes["value"] != null)
                    {
                        key = childNode.Attributes["key"].Value;
                        if (key == AzureLoggingConfiguration.LOGGER_NAME)
                        {
                            config.AzureLoggerName = childNode.Attributes["value"].Value;
                        }
                        if (key == AzureLoggingConfiguration.CONNECTION_STRING)
                        {
                            config.AzureLoggingStorageAccountConnection = childNode.Attributes["value"].Value;
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
