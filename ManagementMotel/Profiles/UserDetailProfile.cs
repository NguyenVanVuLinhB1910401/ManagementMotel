using AutoMapper;
using ManagementMotel.DTOs;
using ManagementMotel.Models;

namespace ManagementMotel.Profiles
{
    public class UserDetailProfile : Profile
    {
        public UserDetailProfile() 
        {
            CreateMap<RegisterDto, UserDetail>()
                 .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.CCCD, opt => opt.MapFrom(src => src.CCCD))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.MobilePhone, opt => opt.MapFrom(src => src.MobilePhone))
                .ForMember(dest => dest.ImagePreviousCCCD, opt => opt.MapFrom(src => src.ImagePreviousCCCD.FileName))
                .ForMember(dest => dest.ImageAfterCCCD, opt => opt.MapFrom(src => src.ImageAfterCCCD.FileName))
                .ForMember(dest => dest.User, opt => opt.Ignore());
        }
    }
}
