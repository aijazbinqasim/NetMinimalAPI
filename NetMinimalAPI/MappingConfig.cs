using AutoMapper;
using NetMinimalAPI.Models;
using NetMinimalAPI.Models.Dtos;

namespace NetMinimalAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig() 
        {
            CreateMap<Coupon, PostCouponDto>().ReverseMap();
            CreateMap<Coupon, GetCouponDto>().ReverseMap();
        }
    }
}
