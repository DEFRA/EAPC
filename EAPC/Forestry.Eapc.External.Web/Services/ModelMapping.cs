using AutoMapper;
using Forestry.Eapc.External.Web.Models.Profile;
using Forestry.Eapc.External.Web.Services.Repositories.Users;

namespace Forestry.Eapc.External.Web.Services
{
    public static class ModelMapping
    {
        private static readonly IMapper Mapper;

        static ModelMapping()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ExternalUser, UserProfileModel>()
                    .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.GivenName))
                    .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Surname))
                    .ForMember(dest => dest.AddressLine1, opt => opt.MapFrom(src => src.StreetAddressLine1))
                    .ForMember(dest => dest.AddressLine2, opt => opt.MapFrom(src => src.StreetAddressLine2))
                    .ForMember(dest => dest.AddressLine3, opt => opt.MapFrom(src => src.StreetAddressLine3))
                    .ForMember(dest => dest.AddressLine4, opt => opt.MapFrom(src => src.StreetAddressLine4))
                    .ForMember(dest => dest.TelephoneNumber, opt => opt.MapFrom(src => src.Telephone))
                    .ForMember(dest => dest.CreditReferenceNumber, opt => opt.MapFrom(src => src.CreditAccountReference))
                    .ForMember(dest => dest.AcceptsCreditTermsAndConditions, opt => opt.MapFrom(src => src.SignedUpToCreditTermsAndConditions));
            });

            Mapper = config.CreateMapper();
        }
        
        public static UserProfileModel ToUserProfileModel(ExternalUser user)
        {
            return Mapper.Map<UserProfileModel>(user);
        }
    }
}
