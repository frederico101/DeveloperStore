using AutoMapper;
using MediatR;
using SalesApi.Application.Commands;
using SalesApi.Application.DTOs;
using Structure;
using Domain;

namespace SalesApi.Application.Handlers
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
    {
        private readonly SalesDbContext _context;
        private readonly IMapper _mapper;

        public CreateProductCommandHandler(SalesDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var title = request.Title == "" ? "Default Title" : request.Title;

            // Map the incoming command to the Product entity
            var product = _mapper.Map<Product>(request);

            // Add the product to the database
            _context.Products.Add(product);
            await _context.SaveChangesAsync(cancellationToken);

            // Map the Product entity to the ProductDto
            var productDto = _mapper.Map<ProductDto>(product);
            return productDto;
        }
    }
}