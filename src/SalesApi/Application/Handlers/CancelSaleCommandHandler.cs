using MediatR;
using SalesApi.Application.Commands;
using Structure;

namespace SalesApi.Application.Handlers
{
    public class CancelSaleCommandHandler : IRequestHandler<CancelSaleCommand, Unit>
    {
        private readonly SalesDbContext _context;

        public CancelSaleCommandHandler(SalesDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = await _context.Sales.FindAsync(request.SaleId);
            if (sale == null)
            {
                throw new KeyNotFoundException("Sale not found");
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}