using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCC.UtilityFramework.Loggings.AzureLogs
{
    public interface IAzureLoggingConfiguration
    {
        bool NeedAzureLogging
        {
            get; 
        }

        string AzureLoggingStorageAccountConnection
        {
            get;
            set;
        }

        string AzureLoggerName
        {
            get;
            set;
        } 
    }
}
