using MediatR;

namespace SalesApi.Application.Commands;

public class CreateSaleCommand : IRequest<Guid>
{
    public string SaleNumber { get; set; }
    public DateTime SaleDate { get; set; }
    public string CustomerId { get; set; }
    public string BranchId { get; set; }
    public List<SaleItem> Items { get; set; }

    public CreateSaleCommand(string saleNumber, string customerId, string branchId, List<SaleItem> items)
    {
        SaleNumber = saleNumber;
        CustomerId = customerId;
        BranchId = branchId;
        Items = items;
    }
}