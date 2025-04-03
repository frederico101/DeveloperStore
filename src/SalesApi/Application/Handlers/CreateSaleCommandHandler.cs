using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesApi.Application.Commands;
using SalesApi.Application.DTOs;
using Structure;


namespace SalesApi.Application.Handlers;

public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, SaleDto>
{
    private readonly SalesDbContext _context;
    private readonly IMapper _mapper;
    public CreateSaleCommandHandler(SalesDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<SaleDto> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {


        //var sale = new Sale(request.SaleNumber, request.CustomerId, request.BranchId, request.Items);
      


        var saleItems = new List<SaleItem>();
        foreach (var item in request.Items)
        {
            var saleItem = new SaleItem(item.ProductId, item.Quantity, item.UnitPrice, item.Discount);
            saleItems.Add(saleItem);
        }

        var sale = new Sale(request.SaleNumber, request.CustomerId, request.BranchId, saleItems);
        _context.Sales.Add(sale);
        await _context.SaveChangesAsync(cancellationToken);

        var saleDto = _mapper.Map<SaleDto>(sale);
        return saleDto;
    }
}