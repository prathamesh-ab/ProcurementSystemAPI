using ProcurementSystem.API.Interfaces;
using ProcurementSystem.API.Models;
using System.Data;
using System.Data.SqlClient;

namespace ProcurementSystem.API.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        public SupplierRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        //public async Task<List<Supplier>> GetAllAsync() {
        public async Task<List<Supplier>> GetAllAsync()
        {

            var suppliers = new List<Supplier>();
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = @"SELECT * FROM Suppliers WHERE IsActive = 1 ORDER BY SupplierName";

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        suppliers.Add(MapReaderToSupplier(reader));
                    }
                }
            }
            return suppliers;
        }

        public async Task<Supplier> GetByIdAsync(int id)
        {

            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = @"SELECT * FROM Suppliers WHERE SupplierId = @Id AND IsActive = 1";
                command.Parameters.Add(new SqlParameter("@Id", id));

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return MapReaderToSupplier(reader);
                    }
                }
            }
            return null;
        }

        public async Task<int> CreateAsync(Supplier supplier)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = @"INSERT INTO SUPPLIERS(SupplierName,ContactEmail,ContactPhone,Country,Rating)
                                        VALUES(@SupplierName,@ContactEmail,@ContactPhone,@Country,@Rating);
                                        SELECT CAST(SCOPE_IDENTITY() as int);";
                command.Parameters.Add(new SqlParameter("@SupplierName", supplier.SupplierName));
                command.Parameters.Add(new SqlParameter("@ContactEmail", supplier.ContactEmail));
                command.Parameters.Add(new SqlParameter("@ContactPhone", (object)supplier.ContactPhone ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@Country", (object)supplier.Country ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@Rating", (object)supplier.Rating ?? DBNull.Value));

                return (int)await command.ExecuteScalarAsync();
            }
        }

        public async Task<bool> UpdateAsync(Supplier supplier)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = @"UPDATE SUPPLIERS 
                                        SET SupplierName = @SupplierName,
                                        ContactEmail = @ContactEmail,
                                        ContactPhone = @ContactPhone,
                                        Country = @Country,
                                        Rating = @Rating,
                                        UpdatedAt = GETDATE()   
                                        WHERE SupplierId = @Id AND IsActive = 1";
                command.Parameters.Add(new SqlParameter("@Id", supplier.SupplierId));
                command.Parameters.Add(new SqlParameter("@SupplierName", supplier.SupplierName));
                command.Parameters.Add(new SqlParameter("@ContactEmail", supplier.ContactEmail));
                command.Parameters.Add(new SqlParameter("@ContactPhone", (object)supplier.ContactPhone ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@Country", (object)supplier.Country ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@Rating", (object)supplier.Rating ?? DBNull.Value));

                int rowAffected = await command.ExecuteNonQueryAsync();

                return rowAffected > 0;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = @"UPDATE SUPPLIERS
                                        SET IsActive = 0,
                                        UpdatedAt = GETDATE()
                                        WHERE SupplierId = @Id AND IsActive = 1";
                command.Parameters.Add(new SqlParameter("@Id", id));

                int rowAffected = await command.ExecuteNonQueryAsync();

                return rowAffected > 0;
            }
        }

        public async Task<List<Supplier>> GetByCountryAsync(string country)
        {

            var suppliers = new List<Supplier>();
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = @"SELECT * FROM Suppliers WHERE Country = @Country AND IsActive = 1 ORDER BY Rating DESC";
                command.Parameters.Add(new SqlParameter("@Country", country));

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        suppliers.Add(MapReaderToSupplier(reader));
                    }
                }
            }
            return suppliers;
        }
        public async Task<List<Supplier>> GetTopRatedAsync(int count)
        {

            var suppliers = new List<Supplier>();
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = @"SELECT * FROM Suppliers
                                        WHERE IsActive = 1 AND Rating IS NOT NULL 
                                        ORDER BY Rating DESC
                                        OFFSET 0 ROWS FETCH NEXT @Count ROWS ONLY";
                command.Parameters.Add(new SqlParameter("@Count", count));
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        suppliers.Add(MapReaderToSupplier(reader));
                    }
                }
            }
            return suppliers;
        }



        private Supplier MapReaderToSupplier(IDataReader reader)
        {
            return new Supplier
            {
                SupplierId = reader.GetInt32(reader.GetOrdinal("SupplierId")),
                SupplierName = reader.GetString(reader.GetOrdinal("SupplierName")),
                ContactEmail = reader.GetString(reader.GetOrdinal("ContactEmail")),
                ContactPhone = reader.IsDBNull(reader.GetOrdinal("ContactPhone")) ? null : reader.GetString(reader.GetOrdinal("ContactPhone")),
                Country = reader.IsDBNull(reader.GetOrdinal("Country"))
                    ? null : reader.GetString(reader.GetOrdinal("Country")),
                Rating = reader.IsDBNull(reader.GetOrdinal("Rating"))
                    ? null : reader.GetDecimal(reader.GetOrdinal("Rating")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt"))
                    ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
            };
        }
    }
}
