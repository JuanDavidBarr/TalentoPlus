namespace TalentoPlus.DTOs.Auth
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public string TokenType { get; set; } = "Bearer";
        public DateTime ExpiresAt { get; set; }
        public EmployeeBasicInfo Employee { get; set; }
    }

    public class EmployeeBasicInfo
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}