using MediatR;
using SalesApi.Application.DTOs;

namespace SalesApi.Application.Queries
{
    public class GetSalesQuery : IRequest<List<SaleDto>>
    {
    }
}