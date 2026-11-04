using SerieA.API.Entities;

namespace SerieA.API.Services.Auth
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
