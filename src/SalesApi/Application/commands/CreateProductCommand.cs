using MediatR;
using SalesApi.Application.DTOs;

namespace SalesApi.Application.Commands
{
    public class CreateProductCommand : IRequest<ProductDto>
    {
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Image { get; set; }
    }
}