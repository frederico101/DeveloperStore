using AutoMapper;
using Domain;
using SalesApi.Application.DTOs;

namespace SalesApi.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Sale, SaleDto>()
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.Items.Sum(i => i.Quantity * i.UnitPrice - i.Discount)))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ReverseMap();

            CreateMap<SaleItem, SaleItemDto>()
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Quantity * src.UnitPrice - src.Discount))
                .ReverseMap();
        }
    }
}
