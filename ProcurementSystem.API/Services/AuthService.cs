using Microsoft.IdentityModel.Tokens;
using ProcurementSystem.API.DTOs;
using ProcurementSystem.API.Interfaces;
using ProcurementSystem.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static ProcurementSystem.API.Exceptions.CustomExceptions;

namespace ProcurementSystem.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IAuthRepository repository,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _repository = repository;
            _configuration = configuration;
            _logger = logger;
        }


        public async Task<AuthResponseDTO> LoginAsync(LoginDTO dto)
        {
            try
            {
                _logger.LogInformation("Login attempt for email: {Email}", dto.Email);

                // Find user
                var user = await _repository.GetUserByEmailAsync(dto.Email);

                if (user == null)
                {
                    _logger.LogWarning("Login failed: User not found - {Email}", dto.Email);
                    throw new NotFoundException("Invalid email or password");
                }

                // Verify password
                if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Login failed: Invalid password - {Email}", dto.Email);
                    throw new NotFoundException("Invalid email or password");
                }

                // Update last login
                await _repository.UpdateLastLoginAsync(user.UserId);

                // Generate JWT token
                var token = GenerateJwtToken(user);

                _logger.LogInformation("Login successful for user: {Email}", dto.Email);

                return new AuthResponseDTO
                {
                    Token = token,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(
                        _configuration.GetValue<int>("JwtSettings:ExpirationMinutes"))
                };
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Email}", dto.Email);
                throw new DatabaseException("Login failed", ex);
            }
        }

        public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO dto)
        {
            try
            {
                _logger.LogInformation("Registration attempt for email: {Email}", dto.Email);

                // Check if email exists
                if (await _repository.EmailExistsAsync(dto.Email))
                {
                    throw new DuplicateException("Email already registered");
                }

                // Check if username exists
                if (await _repository.UsernameExistsAsync(dto.Username))
                {
                    throw new DuplicateException("Username already taken");
                }

                // Hash password
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

                // Create user
                var user = new User
                {
                    Username = dto.Username,
                    Email = dto.Email,
                    PasswordHash = passwordHash,
                    FullName = dto.FullName,
                    Role = dto.Role == "Admin" ? "User" : dto.Role // Prevent self-admin creation
                };

                var userId = await _repository.CreateUserAsync(user);
                user.UserId = userId;

                // Generate token
                var token = GenerateJwtToken(user);

                _logger.LogInformation("User registered successfully: {Email}", dto.Email);

                return new AuthResponseDTO
                {
                    Token = token,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(
                        _configuration.GetValue<int>("JwtSettings:ExpirationMinutes"))
                };
            }
            catch (DuplicateException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for {Email}", dto.Email);
                throw new DatabaseException("Registration failed", ex);
            }
        }

        private string GenerateJwtToken(User user)
        {
            var secretKey = _configuration["JwtSettings:SecretKey"];
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];
            var expirationMinutes = _configuration.GetValue<int>("JwtSettings:ExpirationMinutes");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("FullName", user.FullName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
