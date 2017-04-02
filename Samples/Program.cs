using Autofac;
using Autofac.Core;
using SharpCC.UtilityFramework.AzureStorage;
using SharpCC.UtilityFramework.Loggings;
using SharpCC.UtilityFramework.Loggings.AzureLogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCC.UtilityFramework.Samples
{
    class Program
    {
        private static IContainer Container { get; set; }

        static void Main(string[] args)
        {
            AppConfigAzureCloudStorageConfiguration m_azureConfiguration = null;
            try
            {
                m_azureConfiguration =
                    System.Configuration.ConfigurationManager
                    .GetSection("azureStorages") as AppConfigAzureCloudStorageConfiguration;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
            AppConfigAzureLoggingConfiguration azureLoggingConfiguration = null;
            try
            {
                azureLoggingConfiguration = System.Configuration.ConfigurationManager
                    .GetSection("azureLoggings")
                    as AppConfigAzureLoggingConfiguration;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }

            var builder = new ContainerBuilder();
            builder.RegisterType<AppConfigAzureCloudStorageConfiguration>()
                .As<IAzureCloudStorageConfiguration>();
            builder.RegisterType<AzureCloudStorageContext>();
            builder.RegisterInstance<IAzureCloudStorageConfiguration>(m_azureConfiguration);

            builder.RegisterType<AppConfigLog4NetConfiguration>()
                .As<ILog4NetConfiguration>();
            builder.RegisterType<AzureTableLogTarget>();
            builder.RegisterType<AppConfigAzureLoggingConfiguration>()
                .As<IAzureLoggingConfiguration>();
            AppConfigLog4NetConfiguration appconfigLog4J =
                new AppConfigLog4NetConfiguration("test test");
            builder.RegisterInstance<ILog4NetConfiguration>(appconfigLog4J);
            builder.RegisterInstance<AppConfigAzureLoggingConfiguration>(azureLoggingConfiguration); 
            builder.RegisterType<DebugConsoleLogTarget>();
            builder.RegisterType<ConsoleAndAzureTableLogStrategy>()
                .As<ILogStrategy>(); 
            
            Container = builder.Build();
            LogHelper.AutofacContainer = Container;

            WriteDate();
        }

        public static void WriteDate()
        { 
            
            // Create the scope, resolve your IDateWriter,
            // use it, then dispose of the scope.
            using (var scope = Container.BeginLifetimeScope())
            {
                LogHelper.Debug("test");
                //LogHelperImpl impl = scope.Resolve<LogHelperImpl>();
                AzureCloudStorageContext context
                    = scope.Resolve<AzureCloudStorageContext>();
                Console.WriteLine(context.ToString());
            }
        }
    }
}
