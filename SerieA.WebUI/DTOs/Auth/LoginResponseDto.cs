using SerieA.WebUI.Enums;

namespace SerieA.WebUI.DTOs.Auth
{
    public class LoginResponseDto
    {
        public Guid Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; }
    }
}
