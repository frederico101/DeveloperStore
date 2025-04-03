using MediatR;
using SalesApi.Application.Commands;
using SalesApi.Application.DTOs;

namespace SalesApi.Application.Handlers
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
    {
        private static readonly List<ProductDto> Products = new();

        public Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new ProductDto
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Price = request.Price,
                Description = request.Description,
                Category = request.Category,
                Image = request.Image
            };

            Products.Add(product);
            return Task.FromResult(product);
        }
    }
}