using SerieA.WebUI.DTOs.Users;
using SerieA.WebUI.Enums;

namespace SerieA.WebUI.ViewModels
{
    public class UserIndexViewModel
    {
        public List<ResultUserDto> Users { get; set; } = new();
        // Active Filter Statuses
        public string? Query { get; set; }
        public UserRole? Role { get; set; }
        public bool? Active { get; set; }
    }
}
