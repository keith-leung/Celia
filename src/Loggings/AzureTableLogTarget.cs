using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using SharpCC.UtilityFramework.Loggings.AzureLogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCC.UtilityFramework.Loggings
{
    public class AzureTableLogTarget : ILogTarget
    {
        public AzureTableLogTarget(IAzureLoggingConfiguration configuration)
        {
            this.m_configuration = configuration;
            this.Init();
        }

        private void Init()
        {
            if (m_configuration != null && m_configuration.NeedAzureLogging)
            {
                this.AzureLoggingStorageAccount = CloudStorageAccount.Parse(
                    this.m_configuration.AzureLoggingStorageAccountConnection);
                this.AzureTableClient = AzureLoggingStorageAccount.CreateCloudTableClient();
            }
        }

        private IAzureLoggingConfiguration m_configuration;

        public bool NeedAzureTableLogging
        {
            get
            {
                if (m_configuration != null && m_configuration.NeedAzureLogging)
                {
                    return AzureLoggingStorageAccount != null && AzureTableClient != null;
                }
                return false;
            }
        }

        public CloudStorageAccount AzureLoggingStorageAccount { get; internal set; }

        public CloudTableClient AzureTableClient
        {
            get; internal set;
        }

        public void Debug(string p)
        {
            if (!NeedAzureTableLogging)
                return;
            AppendToAzureTable(AzureLogs.AzureLogType.Debug, p, null);
        }

        public void Error(string p, Exception e)
        {
            if (!NeedAzureTableLogging)
                return;
            AppendToAzureTable(AzureLogs.AzureLogType.Error, p, e);
        }

        public void Error(string p)
        {
            if (!NeedAzureTableLogging)
                return;
            AppendToAzureTable(AzureLogs.AzureLogType.Error, p, null);
        }

        public void Fatal(string p, Exception e)
        {
            if (!NeedAzureTableLogging)
                return;
            AppendToAzureTable(AzureLogs.AzureLogType.Fatal, p, e);
        }

        public void Info(string p)
        {
            if (!NeedAzureTableLogging)
                return;
            AppendToAzureTable(AzureLogs.AzureLogType.Info, p, null);
        }

        public void Warn(string p)
        {
            if (!NeedAzureTableLogging)
                return;
            AppendToAzureTable(AzureLogs.AzureLogType.Warn, p, null);
        }

        public void Warn(string p, Exception e)
        {
            if (!NeedAzureTableLogging)
                return;
            AppendToAzureTable(AzureLogs.AzureLogType.Warn, p, e);
        }

        private void AppendToAzureTable(AzureLogType logType,
            string p, Exception ex)
        {
            try
            {
                AzureLogs.AzureLogTableEntity log = new AzureLogs.AzureLogTableEntity(logType
                    , this.m_configuration.AzureLoggerName)
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
                        var warn = new AzureLogs.AzureLogTableEntity(AzureLogs.AzureLogType.Warn
                            , this.m_configuration.AzureLoggerName)
                        {
                            Message = p + "\tAggregationExceptions: "
                        };
                        list.Add(warn);

                        if (aggr != null && aggr.InnerExceptions != null &&
                            aggr.InnerExceptions.Count > 0)
                        {
                            foreach (var er in aggr.InnerExceptions)
                            {
                                list.Add(new AzureLogs.AzureLogTableEntity(AzureLogs.AzureLogType.Error
                                , this.m_configuration.AzureLoggerName)
                                { Message = er.Message });
                            }
                        }
                        else if (aggr != null && aggr.InnerException != null)
                        {
                            list.Add(new AzureLogs.AzureLogTableEntity(AzureLogs.AzureLogType.Error
                            , this.m_configuration.AzureLoggerName)
                            { Message = aggr.InnerException.Message });
                        }
                        warn = new AzureLogs.AzureLogTableEntity(AzureLogs.AzureLogType.Warn
                        , this.m_configuration.AzureLoggerName)
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
    }
}
