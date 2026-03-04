using ProcurementSystem.API.Interfaces;
using ProcurementSystem.API.Models;
using System.Data;
using System.Data.SqlClient;

namespace ProcurementSystem.API.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public AuthRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = @"SELECT * FROM Users WHERE Email = @Email AND IsActive = 1";
                command.Parameters.Add(new SqlParameter("@Email", email));

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return MapReaderToUser(reader);
                    }
                }
            }
            return null;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = @"SELECT * FROM Users WHERE Username = @Username AND IsActive = 1";
                command.Parameters.Add(new SqlParameter("@Username", username));

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return MapReaderToUser(reader);
                    }
                }
            }
            return null;
        }

        public async Task<int> CreateUserAsync(User user)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = @"INSERT INTO USERS(Username,Email,PasswordHash,FullName,Role)
                                        VALUES(@Username,@Email,@PasswordHash,@FullName,@Role);
                                        SELECT CAST(SCOPE_IDENTITY() as int);";
                command.Parameters.Add(new SqlParameter("@Username", user.Username));
                command.Parameters.Add(new SqlParameter("@Email", user.Email));
                command.Parameters.Add(new SqlParameter("@PasswordHash", user.PasswordHash));
                command.Parameters.Add(new SqlParameter("@FullName", user.FullName));
                command.Parameters.Add(new SqlParameter("@Role", user.Role));

                return (int)await command.ExecuteScalarAsync();
            }
        }

        public async Task UpdateLastLoginAsync(int userId)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = @"UPDATE USERS SET LastLoginAt = GETDATE() WHERE UserId = @UserId";
                command.Parameters.Add(new SqlParameter("@UserId", userId));

                await command.ExecuteNonQueryAsync();

            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = @"SELECT COUNT(*) FROM USERS WHERE Email = @Email";
                command.Parameters.Add(new SqlParameter("@Email", email));

                int count =  (int)await command.ExecuteScalarAsync();
                return count > 0;

            }
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = @"SELECT COUNT(*) FROM USERS WHERE Username = @Username";
                command.Parameters.Add(new SqlParameter("@Username", username));

                int count = (int)await command.ExecuteScalarAsync();
                return count > 0;

            }
        }

        private User MapReaderToUser(IDataReader reader)
        {
            return new User
            {
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                Username = reader.GetString(reader.GetOrdinal("Username")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                FullName = reader.GetString(reader.GetOrdinal("FullName")),
                Role = reader.GetString(reader.GetOrdinal("Role")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                LastLoginAt = reader.IsDBNull(reader.GetOrdinal("LastLoginAt")) ? null : reader.GetDateTime(reader.GetOrdinal("LastLoginAt"))
            };
        }
    }
}
