using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SharpCC.UtilityFramework.DocumentDBs
{
    public class DocumentDbConnectionConfigurationSectionHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            DocumentDbConnectionConfiguration config = new DocumentDbConnectionConfiguration();

            if (section.Name.Equals(DocumentDbConnectionConfiguration.CONNECTION_STRINGS,
                StringComparison.InvariantCultureIgnoreCase))
            {
                foreach (XmlNode childNode in section.ChildNodes)
                {
                    if (!childNode.Name.Equals(DocumentDbConnectionConfiguration.CONNECTION_STRING_NODE_NAME,
                        StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    try
                    {
                        string key = string.Empty;
                        string runtime = string.Empty;

                        if (childNode.Attributes[DocumentDbConnectionConfiguration.ACCOUNT_ENDPOINT] != null
                            && childNode.Attributes[DocumentDbConnectionConfiguration.ACCOUNT_KEY] != null)
                        {
                            if (childNode.Attributes[DocumentDbConnectionConfiguration.CONNECTION_STRING_NODE_KEY] != null)
                            {
                                key = childNode.Attributes[DocumentDbConnectionConfiguration.CONNECTION_STRING_NODE_KEY].Value;
                            }

                            ConfigSectionRuntimeEnum rt = ConfigSectionRuntimeEnum.RELEASE;
                            if (childNode.Attributes[DocumentDbConnectionConfiguration.RUNTIME] != null)
                            {
                                runtime = childNode.Attributes[DocumentDbConnectionConfiguration.RUNTIME].Value;
                                if (!string.IsNullOrEmpty(runtime) && runtime.Equals(
                                    DocumentDbConnectionConfiguration.RUNTIME_DEBUG, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    rt = ConfigSectionRuntimeEnum.DEBUG;
                                }
                                else if (!string.IsNullOrEmpty(runtime) && runtime.Equals(
                                   DocumentDbConnectionConfiguration.RUNTIME_FORCE, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    rt = ConfigSectionRuntimeEnum.FORCE;
                                }
                            }

                            string databaseName = string.Empty;
                            string collectionDefault = string.Empty;
                            if (childNode.Attributes[DocumentDbConnectionConfiguration.DATABASENAME] != null)
                            {
                                databaseName = childNode.Attributes[DocumentDbConnectionConfiguration.DATABASENAME].Value;
                            }
                            if (childNode.Attributes[DocumentDbConnectionConfiguration.COLLECTIONDEFAULT] != null)
                            {
                                collectionDefault = childNode.Attributes[DocumentDbConnectionConfiguration.COLLECTIONDEFAULT].Value;
                            }

                            if (!config.ConnectionStrings.Any(m => m.Key.Equals(key)))
                            {
                                config.ConnectionStrings.Add(new DocumentDbConnectionString()
                                {
                                    Runtime = rt,
                                    Key = key,
                                    DataBaseName = databaseName,
                                    CollectionDefault = collectionDefault,
                                    AccountKey = childNode.Attributes[DocumentDbConnectionConfiguration.ACCOUNT_KEY].Value,
                                    AccountEndpoint = childNode.Attributes[DocumentDbConnectionConfiguration.ACCOUNT_ENDPOINT].Value,
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        System.Diagnostics.Trace.TraceError(ex.Message);
                    }
                }
            }

            //config.Build();

            return config;
        }
    }
}
