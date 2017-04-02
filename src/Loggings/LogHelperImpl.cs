using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCC.UtilityFramework.Loggings
{
    internal class LogHelperImpl : ILogStrategy
    {
        public LogHelperImpl(ILogStrategy strategy)
        {
            this.LogStrategy = strategy;
        }

        public ILogStrategy LogStrategy { get;set; }

        public void Error(string p, Exception e)
        {
            if (LogStrategy != null)
                LogStrategy.Error(p, e);
        }

        public void Error(string p)
        {
            if (LogStrategy != null)
                LogStrategy.Error(p);
        }

        public void Info(string p)
        {
            if (LogStrategy != null)
                LogStrategy.Info(p);
        }

        public void Debug(string p)
        {
            if (LogStrategy != null)
                LogStrategy.Debug(p);
        }

        public void Fatal(string p, Exception e)
        {
            if (LogStrategy != null)
                LogStrategy.Fatal(p, e);
        }

        public void Warn(string p)
        {
            if (LogStrategy != null)
                LogStrategy.Warn(p);
        }

        public void Warn(string p, Exception e)
        {
            if (LogStrategy != null)
                LogStrategy.Warn(p, e);
        }
    }
}
