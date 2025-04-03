using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesApi.Application.Commands;
using Structure;


namespace SalesApi.Application.Handlers;

public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, Guid>
{
    private readonly SalesDbContext _context;

    public CreateSaleCommandHandler(SalesDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        

        var sale = new Sale(request.SaleNumber, request.CustomerId, request.BranchId, request.Items);

       var saleItems = new List<SaleItem>();
        foreach (var item in request.Items)
        {
            var saleItem = new SaleItem(item.ProductId, item.ProductName, item.Quantity, item.UnitPrice, item.Discount);
            saleItems.Add(saleItem);
        }

        _context.Sales.Add(sale); // Only add Sale; EF will track SaleItems automatically
        await _context.SaveChangesAsync(cancellationToken);

        return sale.SaleId;
    }
}