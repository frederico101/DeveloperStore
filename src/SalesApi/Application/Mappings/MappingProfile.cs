using AutoMapper;
using Domain;
using SalesApi.Application.DTOs;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Sale, SaleDto>()
        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.SaleId))
        .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.SaleDate))
        .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer))
        .ForMember(dest => dest.Branch, opt => opt.MapFrom(src => src.Branch))
        .ForMember(dest => dest.Cancelled, opt => opt.MapFrom(src => src.IsCancelled));

        CreateMap<SaleItem, SaleItemDto>()
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.TotalAmount))
            .ForMember(dest => dest.IsCancelled, opt => opt.MapFrom(src => false));
    }
}
