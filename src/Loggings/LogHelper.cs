using Autofac;
using log4net;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SharpCC.UtilityFramework.Loggings
{
    public class LogHelper
    {
        private static IContainer m_AutofacContainer;

        public static IContainer AutofacContainer
        {
            get { return m_AutofacContainer; }
            set
            {
                m_AutofacContainer = value;
                if (m_AutofacContainer != null
                    && !m_AutofacContainer.IsRegistered(typeof(LogHelperImpl)))
                {
                    m_AutofacContainer.
                }
            }
        }

        class Factory
        {
            public static LogHelperImpl CreateInstance()
            {
                if (AutofacContainer != null)
                {
                    using (var scope = AutofacContainer.BeginLifetimeScope())
                    {
                        return scope.Resolve<LogHelperImpl>();
                    }
                }
                return new LogHelperImpl(null);
            }
        }

        public static void Error(string p, Exception e)
        {
            LogHelperImpl impl = Factory.CreateInstance();//new LogHelperImpl();
            impl.Error(p, e);
        }

        public static void Error(string p)
        {
            LogHelperImpl impl = Factory.CreateInstance();
            impl.Error(p);
        }

        public static void Info(string p)
        {
            LogHelperImpl impl = Factory.CreateInstance();
            impl.Info(p);
        }

        public static void Debug(string p)
        {
            LogHelperImpl impl = Factory.CreateInstance();
            impl.Debug(p);
        }

        public static void Fatal(string p, Exception e)
        {
            LogHelperImpl impl = Factory.CreateInstance();
            impl.Fatal(p, e);
        }

        public static void Warn(string p)
        {
            LogHelperImpl impl = Factory.CreateInstance();
            impl.Warn(p);
        }

        public static void Warn(string p, Exception e)
        {
            LogHelperImpl impl = Factory.CreateInstance();
            impl.Warn(p, e);
        }
    }
}