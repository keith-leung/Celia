using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCC.UtilityFramework.DocumentDBs
{
    internal class DocumentDbConnectionString
    {
        public ConfigSectionRuntimeEnum Runtime
        {
            get;
            internal set;
        }

        /// <summary>
        /// 标志连接字符串的KEY
        /// </summary>
        public string Key { get; internal set; }

        /// <summary>
        /// DocumentDB的EndPoint URL
        /// </summary>
        public string AccountEndpoint { get; internal set; }
        /// <summary>
        /// DocumentDB的AccountKey
        /// </summary>
        public string AccountKey { get; internal set; }

        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DataBaseName { get; set; }
        /// <summary>
        /// 默认集合
        /// </summary>
        public string CollectionDefault { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}]|[{1}] AccountEndpoint={2};AccountKey={3}; {4}@{5}",
                this.Key, this.Runtime.ToString(), this.AccountEndpoint, this.AccountKey,
                this.CollectionDefault, this.DataBaseName);
        }
    }
}
