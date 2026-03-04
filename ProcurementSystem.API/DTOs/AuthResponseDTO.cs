namespace ProcurementSystem.API.DTOs
{
    public class AuthResponseDTO
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
