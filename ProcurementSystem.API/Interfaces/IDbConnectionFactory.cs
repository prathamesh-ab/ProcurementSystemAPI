using System.Data;
using System.Data.Common;

namespace ProcurementSystem.API.Interfaces
{
    public interface IDbConnectionFactory
    {
        public DbConnection CreateConnection();
    }
}
