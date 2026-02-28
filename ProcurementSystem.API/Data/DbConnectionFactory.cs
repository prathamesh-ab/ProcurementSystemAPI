using ProcurementSystem.API.Interfaces;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace ProcurementSystem.API.Data
{
    public class DbConnectionFactory:IDbConnectionFactory
    {
        private readonly string _connectionString;
        public DbConnectionFactory(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public DbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
