using SerieA.API.Enums;

namespace SerieA.API.DTOs.Users
{
    public class CreateUserDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Editor;
        public bool Status { get; set; } = true;
    }
}
