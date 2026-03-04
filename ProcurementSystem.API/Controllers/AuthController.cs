using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProcurementSystem.API.DTOs;
using ProcurementSystem.API.Interfaces;
using System.Data.SqlClient;

namespace ProcurementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService,IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            var response = await _authService.LoginAsync(dto);

            return Ok(new ApiResponse<AuthResponseDTO>
            {
                Success = true,
                Message = "Login successful",
                Data = response
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            var response = await _authService.RegisterAsync(dto);

            return Ok(new ApiResponse<AuthResponseDTO>
            {
                Success = true,
                Message = "Registration successful",
                Data = response
            });
        }

        [HttpPost("create-admin")]
        [AllowAnonymous] // ⚠️ REMOVE THIS ENDPOINT AFTER CREATING ADMIN!
        public async Task<IActionResult> CreateAdmin([FromBody] RegisterDTO dto)
        {
            // Hash the password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // Directly insert admin into database
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand(@"
            INSERT INTO Users (Username, Email, PasswordHash, FullName, Role)
            VALUES (@Username, @Email, @PasswordHash, @FullName, 'Admin');
            SELECT CAST(SCOPE_IDENTITY() as int);", connection);

                command.Parameters.AddWithValue("@Username", dto.Username);
                command.Parameters.AddWithValue("@Email", dto.Email);
                command.Parameters.AddWithValue("@PasswordHash", passwordHash);
                command.Parameters.AddWithValue("@FullName", dto.FullName);

                var userId = (int)await command.ExecuteScalarAsync();

                return Ok(new { Message = $"Admin user created with ID: {userId}" });
            }
        }
    }
}
