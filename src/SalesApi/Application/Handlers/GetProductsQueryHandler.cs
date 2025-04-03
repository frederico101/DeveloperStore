using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesApi.Application.DTOs;
using SalesApi.Application.Queries;
using Structure;

namespace SalesApi.Application.Handlers
{
    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<ProductDto>>
    {
        private readonly SalesDbContext _context;
        private readonly IMapper _mapper;

        public GetProductsQueryHandler(SalesDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _context.Products.ToListAsync(cancellationToken);
            return _mapper.Map<List<ProductDto>>(products);
        }
    }
}