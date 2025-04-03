using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesApi.Application.DTOs;
using SalesApi.Application.Queries;
using Structure;

namespace SalesApi.Application.Handlers
{
    public class GetSalersQueryHandler : IRequestHandler<GetSalesQuery, List<SaleDto>>
    {
        private readonly SalesDbContext _context;
        private readonly IMapper _mapper;

        public GetSalersQueryHandler(SalesDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<SaleDto>> Handle(GetSalesQuery request, CancellationToken cancellationToken)
        {
            var sales = await _context.Sales
                .Include(s => s.Items) 
                .ToListAsync(cancellationToken);
            return _mapper.Map<List<SaleDto>>(sales);
        }
    }
}