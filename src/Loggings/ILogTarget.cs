using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCC.UtilityFramework.Loggings
{
    public interface ILogTarget
    {
        void Error(string p, Exception e);

        void Error(string p);

        void Info(string p);

        void Debug(string p);

        void Fatal(string p, Exception e);

        void Warn(string p);

        void Warn(string p, Exception e);
    }
}
