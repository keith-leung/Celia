using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCC.UtilityFramework.Loggings.AzureLogs
{
    public class AzureLogTableEntity : TableEntity
    {
        public AzureLogTableEntity(AzureLogType logType, string loggerName)
        {
            this.LogType = logType.ToString();
            this.CTIME = DateTime.Now;
            this.PartitionKey = loggerName;// //"Log" + logType.ToString() + this.CTIME.ToString("yyyyMMddHHmmss");
            this.RowKey = this.CTIME.ToString("yyyyMMddHHmmss") + " "
                + Environment.TickCount.ToString() + "_" + Guid.NewGuid().ToString();
        }

        public DateTime CTIME { get; set; }
        public Exception Exception
        {
            get { return m_exception; }
            set
            {
                m_exception = value;
                if (m_exception == null)
                    return;

                this.ExceptionMessage = m_exception.Message + "\r\n" + m_exception.StackTrace;
            }
        }

        private Exception m_exception;

        public string Message { get; set; }
        public string ExceptionMessage { get; set; }
        public string LogType { get; set; }
    }
}
