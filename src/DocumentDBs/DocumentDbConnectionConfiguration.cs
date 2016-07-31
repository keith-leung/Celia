using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCC.UtilityFramework.DocumentDBs
{
    public class DocumentDbConnectionConfiguration
    {
        public const string CONNECTION_STRINGS = "documentDbConnections";

        public const string CONNECTION_STRING_NODE_NAME = "add";

        public const string CONNECTION_STRING_NODE_KEY = "key";

        public const string ACCOUNT_ENDPOINT = "AccountEndpoint";
        public const string ACCOUNT_KEY = "AccountKey";

        public const string RUNTIME = "Runtime";

        public const string DATABASENAME = "DataBaseName";
        public const string COLLECTIONDEFAULT = "CollectionDefault";

        public const string RUNTIME_DEBUG = "Debug";
        public const string RUNTIME_RELEASE = "Release";
        public const string RUNTIME_FORCE = "Force";

        //URI = https://keithdocumentdb.documents.azure.com:443/
        //AccessKey = ht2WC2DgAGHmiUpgkNDFXm1FuZm5PXPsnp3zMYWAyRVSOZxAPuZNdh6fFLvFZqTl9ePgvef4asOnAnGL1ibmPA==

        public DocumentDbConnectionConfiguration()
        {

        }

        internal List<DocumentDbConnectionString> ConnectionStrings
        {
            get { return this.m_connectionStrings; }
        }

        private List<DocumentDbConnectionString> m_connectionStrings = new List<DocumentDbConnectionString>();
    }
}
