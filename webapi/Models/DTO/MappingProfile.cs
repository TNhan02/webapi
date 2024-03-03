using AutoMapper;

namespace webapi.Models.DTO
{
    public class MappingProfile : Profile
    {
        /*
          This is the class for mapping DTO files with the appropriate models
          When a new DTO is added, we must attach a mapping configuration here
        */
        public MappingProfile()
        {
            // User
            CreateMap<DTOUser, User>();
            CreateMap<UserLogin, User>();

            // Product
            CreateMap<DTOProduct, Product>();
        }
    }
}