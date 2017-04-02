using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCC.UtilityFramework.Loggings
{
    public class ConsoleAndAzureTableLogStrategy : ILogStrategy
    {
        IList<ILogTarget> m_targets = new List<ILogTarget>();

        public ConsoleAndAzureTableLogStrategy(
            AzureTableLogTarget azureTableLogTarget,
            DebugConsoleLogTarget debugConsoleLogTarget)
        {
            if (debugConsoleLogTarget != null)
            {
                m_targets.Add(debugConsoleLogTarget);
            }
            if (azureTableLogTarget != null)
            {
                m_targets.Add(azureTableLogTarget);
            }
        }

        public void Error(string p, Exception e)
        {
            foreach (var tg in m_targets)
            {
                try
                {
                    tg.Error(p, e);
                }
                catch (Exception outerEx)
                {
                    System.Diagnostics.Trace.TraceError("ConsoleAndAzureLogStrategy exception on: "
                        + p + " Ex:" + outerEx.Message + "\r\n" + outerEx.StackTrace);
                }
            }
        }

        public void Error(string p)
        {
            foreach (var tg in m_targets)
            {
                try
                {
                    tg.Error(p);
                }
                catch (Exception outerEx)
                {
                    System.Diagnostics.Trace.TraceError("ConsoleAndAzureLogStrategy exception on: "
                        + p + " Ex:" + outerEx.Message + "\r\n" + outerEx.StackTrace);
                }
            }
        }

        public void Info(string p)
        {
            foreach (var tg in m_targets)
            {
                try
                {
                    tg.Info(p);
                }
                catch (Exception outerEx)
                {
                    System.Diagnostics.Trace.TraceError("ConsoleAndAzureLogStrategy exception on: "
                        + p + " Ex:" + outerEx.Message + "\r\n" + outerEx.StackTrace);
                }
            }
        }

        public void Debug(string p)
        {
            foreach (var tg in m_targets)
            {
                try
                {
                    tg.Debug(p);
                }
                catch (Exception outerEx)
                {
                    System.Diagnostics.Trace.TraceError("ConsoleAndAzureLogStrategy exception on: "
                        + p + " Ex:" + outerEx.Message + "\r\n" + outerEx.StackTrace);
                }
            }
        }

        public void Fatal(string p, Exception e)
        {
            foreach (var tg in m_targets)
            {
                try
                {
                    tg.Fatal(p, e);
                }
                catch (Exception outerEx)
                {
                    System.Diagnostics.Trace.TraceError("ConsoleAndAzureLogStrategy exception on: "
                        + p + " Ex:" + outerEx.Message + "\r\n" + outerEx.StackTrace);
                }
            }
        }

        public void Warn(string p)
        {
            foreach (var tg in m_targets)
            {
                try
                {
                    tg.Warn(p, null);
                }
                catch (Exception outerEx)
                {
                    System.Diagnostics.Trace.TraceError("ConsoleAndAzureLogStrategy exception on: "
                        + p + " Ex:" + outerEx.Message + "\r\n" + outerEx.StackTrace);
                }
            }
        }

        public void Warn(string p, Exception e)
        {
            foreach (var tg in m_targets)
            {
                try
                {
                    tg.Warn(p, e);
                }
                catch (Exception outerEx)
                {
                    System.Diagnostics.Trace.TraceError("ConsoleAndAzureLogStrategy exception on: "
                        + p + " Ex:" + outerEx.Message + "\r\n" + outerEx.StackTrace);
                }
            }
        }
    }
}
