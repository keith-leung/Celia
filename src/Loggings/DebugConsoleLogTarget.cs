using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCC.UtilityFramework.Loggings
{
    public class DebugConsoleLogTarget : ILogTarget
    {
        private ILog4NetConfiguration m_config;

        public DebugConsoleLogTarget(ILog4NetConfiguration config)
        {
            m_config = config;
        }

        public void Debug(string p)
        {
            if (m_config != null && !string.IsNullOrEmpty(m_config.LoggerName))
            {
                ILog log = LogManager.GetLogger(m_config.LoggerName);
                log.Debug(p);
            }
        }

        public void Error(string p, Exception e)
        {
            if (m_config != null && !string.IsNullOrEmpty(m_config.LoggerName))
            {
                ILog log = LogManager.GetLogger(m_config.LoggerName);
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
        }

        public void Error(string p)
        {
            if (m_config != null && !string.IsNullOrEmpty(m_config.LoggerName))
            {
                ILog log = LogManager.GetLogger(m_config.LoggerName);
                log.Error(p);
            }
        }

        public void Fatal(string p, Exception e)
        {
            if (m_config != null && !string.IsNullOrEmpty(m_config.LoggerName))
            {
                ILog log = LogManager.GetLogger(m_config.LoggerName);
                log.Fatal(p, e);
            }
        }

        public void Info(string p)
        {
            if (m_config != null && !string.IsNullOrEmpty(m_config.LoggerName))
            {
                ILog log = LogManager.GetLogger(m_config.LoggerName);
                log.Info(p);
            }
        }

        public void Warn(string p)
        {
            if (m_config != null && !string.IsNullOrEmpty(m_config.LoggerName))
            {
                ILog log = LogManager.GetLogger(m_config.LoggerName);
                log.Warn(p);
            }
        }

        public void Warn(string p, Exception e)
        {
            if (m_config != null && !string.IsNullOrEmpty(m_config.LoggerName))
            {
                ILog log = LogManager.GetLogger(m_config.LoggerName);
                log.Warn(p, e);
            }
        }
    }
}
