using MediatR;

namespace SalesApi.Application.Commands
{
    public class CancelSaleCommand : IRequest<Unit>
    {
        public Guid SaleId { get; set; }
    }
}