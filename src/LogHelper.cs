using log4net;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using SharpCC.UtilityFramework.AzureLogs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SharpCC.UtilityFramework
{
    public class LogHelper
    {
        private static string m_loggerName = "DefaultLogger";

        private static bool m_needLog4Net = true;

        #region azure logging
        private static AzureLoggingConfiguration m_azureConfiguration = new AzureLoggingConfiguration();

        public static AzureLoggingConfiguration AzureConfiguration
        {
            get
            {
                return m_azureConfiguration;
            }
        }

        public static bool NeedAzureTableLogging
        {
            get
            {
                if (m_azureConfiguration == null)
                    return false;
                return m_azureConfiguration.NeedAzureLogging;
            }
        }

        public static CloudStorageAccount AzureStorageAccount
        {
            get
            {
                if (m_azureConfiguration != null)
                    return m_azureConfiguration.AzureLoggingStorageAccount;
                return null;
            }
        }

        public static string AzureLoggingStorageAccountConnection
        {
            get
            {
                if (m_azureConfiguration != null)
                    return m_azureConfiguration.AzureLoggingStorageAccountConnection;
                return string.Empty;
            }
            set
            {
                if (m_azureConfiguration == null)
                    m_azureConfiguration = new AzureLoggingConfiguration();
                m_azureConfiguration.AzureLoggingStorageAccountConnection = value;
                m_azureConfiguration.Build();
            }
        }

        public static CloudTableClient AzureTableClient
        {
            get { return m_azureConfiguration.AzureTableClient; }
        }

        public static string AzureLoggerName
        {
            get
            {
                if (NeedAzureTableLogging)
                {
                    return m_azureConfiguration.AzureLoggerName;
                }
                return "DefaultLogger";
            }
            set
            {
                if (NeedAzureTableLogging)
                {
                    m_azureConfiguration.AzureLoggerName = value;
                }
            }
        }
        #endregion

        static LogHelper()
        {
            log4net.Config.XmlConfigurator.Configure();

            try
            {
                m_azureConfiguration = System.Configuration.ConfigurationManager.GetSection("azureLogging") as AzureLoggingConfiguration;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }

            #region change to azure logging configuration
            if (!string.IsNullOrEmpty(
                System.Configuration.ConfigurationManager.AppSettings["Log4NetLoggerName"]))
            {
                m_loggerName = System.Configuration.ConfigurationManager.AppSettings["Log4NetLoggerName"];
            }
            else
            {
                m_needLog4Net = false;
            }
            #endregion
        }

        public static void Error(string p, Exception e)
        {
            if (e != null)
                System.Diagnostics.Trace.TraceError(p + " Ex:" + e.Message + "\r\n" + e.StackTrace);
            else
                System.Diagnostics.Trace.TraceWarning(p);

            ErrorLog4Net(p, e);
            AppendToAzureTable(AzureLogs.AzureLogType.Error, p, e);
        }

        public static void Error(string p)
        {
            System.Diagnostics.Trace.TraceError(p);
            ErrorLog4Net(p);
            AppendToAzureTable(AzureLogs.AzureLogType.Error, p, null);
        }

        public static void Info(string p)
        {
            System.Diagnostics.Trace.TraceInformation(p);
            InfoLog4Net(p);
            AppendToAzureTable(AzureLogs.AzureLogType.Info, p, null);
        }

        public static void Debug(string p)
        {
            System.Diagnostics.Debug.WriteLine(p);
            DebugLog4Net(p);
            AppendToAzureTable(AzureLogs.AzureLogType.Debug, p, null);
        }

        public static void Fatal(string p, Exception e)
        {
            if (e != null)
                System.Diagnostics.Trace.TraceError(p + " Fatal:" + e.Message + "\r\n" + e.StackTrace);
            else
                System.Diagnostics.Trace.TraceWarning(p);

            FatalLog4Net(p, e);
            AppendToAzureTable(AzureLogs.AzureLogType.Fatal, p, null);
        }

        public static void Warn(string p)
        {
            System.Diagnostics.Trace.TraceWarning(p);
            WarnLog4Net(p);
            AppendToAzureTable(AzureLogs.AzureLogType.Warn, p, null);
        }

        public static void Warn(string p, Exception e)
        {
            if (e != null)
                System.Diagnostics.Trace.TraceWarning(p + " Ex:" + e.Message + "\r\n" + e.StackTrace);
            else
                System.Diagnostics.Trace.TraceWarning(p);

            WarnLog4Net(p, e);
            AppendToAzureTable(AzureLogs.AzureLogType.Warn, p, null);
        }

        private static void AppendToAzureTable(AzureLogs.AzureLogType logType, string p, Exception ex)
        {
            if (!NeedAzureTableLogging)
                return;

            try
            {
                AzureLogs.AzureLogTableEntity log = new AzureLogs.AzureLogTableEntity(logType)
                {
                    Message = p,
                    Exception = ex,
                };

                CloudTable table = AzureTableClient.GetTableReference("Log" + log.CTIME.ToString("yyyyMMdd"));
                table.CreateIfNotExists();
                // Create the TableOperation object that inserts the customer entity.
                TableOperation insertOperation = TableOperation.Insert(log);

                // Execute the insert operation.
                table.Execute(insertOperation);

                #region aggregation exception
                try
                {
                    var e = log.Exception;
                    if (e != null && e is AggregateException)
                    {
                        List<AzureLogs.AzureLogTableEntity> list = new List<AzureLogs.AzureLogTableEntity>();
                        AggregateException aggr = e as AggregateException;
                        var warn = new AzureLogs.AzureLogTableEntity(AzureLogs.AzureLogType.Warn)
                        {
                            Message = p + "\tAggregationExceptions: "
                        };
                        list.Add(warn);

                        if (aggr != null && aggr.InnerExceptions != null &&
                            aggr.InnerExceptions.Count > 0)
                        {
                            foreach (var er in aggr.InnerExceptions)
                            {
                                list.Add(new AzureLogs.AzureLogTableEntity(AzureLogs.AzureLogType.Error) { Message = er.Message });
                            }
                        }
                        else if (aggr != null && aggr.InnerException != null)
                        {
                            list.Add(new AzureLogs.AzureLogTableEntity(AzureLogs.AzureLogType.Error) { Message = aggr.InnerException.Message });
                        }
                        warn = new AzureLogs.AzureLogTableEntity(AzureLogs.AzureLogType.Warn)
                        {
                            Message = p + "\tAggregationExceptions Ended. "
                        };
                        list.Add(warn);

                        TableBatchOperation batchOperation = new TableBatchOperation();
                        foreach (var item in list)
                        {
                            batchOperation.Insert(item);
                        }

                        table.ExecuteBatch(batchOperation);
                    }
                }
                catch (Exception innerBatchException)
                {
                    System.Diagnostics.Trace.TraceError(innerBatchException.Message + "\r\n" + innerBatchException.StackTrace);
                }
                #endregion
            }
            catch (Exception exe)
            {
                System.Diagnostics.Trace.TraceError(exe.Message + "\r\n" + exe.StackTrace);
            }
        }

        private static void ErrorLog4Net(string p, Exception e)
        {
            if (!m_needLog4Net)
                return;

            ILog log = LogManager.GetLogger(m_loggerName);
            if (e != null && e is AggregateException)
            {
                AggregateException aggr = e as AggregateException;
                log.Warn(p + "\tAggregationExceptions: ");
                if (aggr != null && aggr.InnerExceptions != null &&
                    aggr.InnerExceptions.Count > 0)
                {
                    foreach (var er in aggr.InnerExceptions)
                    {
                        log.Error(p, er);
                    }
                }
                else if (aggr != null && aggr.InnerException != null)
                {
                    log.Error(p, aggr.InnerException);
                }
                log.Warn(p + "\tAggregationExceptions Ended. ");
            }
            log.Error(p, e);
        }

        private static void ErrorLog4Net(string p)
        {
            if (!m_needLog4Net)
                return;
            ILog log = LogManager.GetLogger(m_loggerName);
            log.Error(p);
        }

        private static void InfoLog4Net(string p)
        {
            if (!m_needLog4Net)
                return;
            ILog log = LogManager.GetLogger(m_loggerName);
            log.Info(p);
        }

        private static void DebugLog4Net(string p)
        {
            if (!m_needLog4Net)
                return;
            ILog log = LogManager.GetLogger(m_loggerName);
            log.Debug(p);
        }

        private static void FatalLog4Net(string p, Exception e)
        {
            if (!m_needLog4Net)
                return;
            ILog log = LogManager.GetLogger(m_loggerName);
            log.Fatal(p, e);
        }

        private static void WarnLog4Net(string p)
        {
            if (!m_needLog4Net)
                return;
            ILog log = LogManager.GetLogger(m_loggerName);
            log.Warn(p);
        }

        private static void WarnLog4Net(string p, Exception e)
        {
            if (!m_needLog4Net)
                return;
            ILog log = LogManager.GetLogger(m_loggerName);
            log.Warn(p, e);
        }
    }
}