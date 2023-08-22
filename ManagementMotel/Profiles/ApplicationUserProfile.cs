using AutoMapper;
using ManagementMotel.DTOs;
using ManagementMotel.Models;

namespace ManagementMotel.Profiles
{
    public class ApplicationUserProfile : Profile
    {
        public ApplicationUserProfile() 
        {
            CreateMap<RegisterDto, ApplicationUser>();
        }
    }
}
