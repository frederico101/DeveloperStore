using MediatR;
using SalesApi.Application.DTOs;

namespace SalesApi.Application.Queries
{
    public class GetProductsQuery : IRequest<List<ProductDto>>
    {
    }
}