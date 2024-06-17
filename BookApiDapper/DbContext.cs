using Microsoft.Data.Sqlite;
using System.Data;
using System.Data.SqlClient;

namespace BookApiDapper
{
    public class DbContext
    {
        private readonly IConfiguration _configuration;

        public DbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateSQLiteConnection()
        {
            var connectionString = _configuration.GetConnectionString("SQLiteConnection");
            return new SqliteConnection(connectionString);
        }

        public IDbConnection CreateSQLServerConnection()
        {
            var connectionString = _configuration.GetConnectionString("SQLServerConnection");
            return new SqlConnection(connectionString);
        }
    }
}
