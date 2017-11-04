using System.Configuration;
using System.Data.Common;
using System.IO;
using System.Reflection;
using Devart.Data.SQLite;

namespace FBFCheckManagement.Infrastructure.EntityFramework
{
    public class EfSQLite : IDatabaseType
    {
        private readonly string _connectionName;

        public EfSQLite(string connectionName)
        {
            _connectionName = connectionName;
        }

        public DbConnection Connectionstring()
        {
            return new SQLiteConnection(ConstrucConnectionString());
        }

        private string ConstrucConnectionString()
        {
            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings[_connectionName];
            var strConnection = connectionStringSettings.ConnectionString;
            var builder = new SQLiteConnectionStringBuilder(strConnection);

            string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            builder.DataSource = appPath + builder.DataSource;
            return builder.ConnectionString;
        }
    }
}