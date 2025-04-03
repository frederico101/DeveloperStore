using MediatR;
using SalesApi.Application.DTOs;

namespace SalesApi.Application.Commands;

public class CreateSaleCommand : IRequest<SaleDto>
{
    public string SaleNumber { get; set; }
    public DateTime SaleDate { get; set; }
    public string CustomerId { get; set; }
    public string BranchId { get; set; }
    public List<SaleItemDto> Items { get; set; }

    public CreateSaleCommand(string saleNumber, string customerId, string branchId, List<SaleItemDto> items)
    {
        SaleNumber = saleNumber;
        CustomerId = customerId;
        BranchId = branchId;
        Items = items;
    }
}