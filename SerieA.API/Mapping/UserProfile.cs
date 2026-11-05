using AutoMapper;
using SerieA.API.DTOs.Auth;
using SerieA.API.DTOs.Users;
using SerieA.API.Entities;

namespace SerieA.API.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, ResultUserDto>();
            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
            CreateMap<User, LoginResponseDto>()
                .ForMember(dest => dest.Token, opt => opt.Ignore());
        }
    }
}
