using AutoMapper;
using Domain;
using SalesApi.Application.Commands;
using SalesApi.Application.DTOs;

namespace SalesApi.Application.Mappings
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<CreateProductCommand, Product>();
            CreateMap<Product, ProductDto>();
        }
    }
}