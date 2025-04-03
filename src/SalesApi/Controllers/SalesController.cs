using Microsoft.AspNetCore.Mvc;
using MediatR;
using SalesApi.Application.Commands;
using SalesApi.Application.Queries;

namespace SalesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SalesController> _logger;

        public SalesController(ILogger<SalesController> logger, IMediator mediator)
        {
            _mediator = mediator;
            _logger = logger;
        }

        //[HttpGet(Name = "GetSales")]
        //public IEnumerable<Sales> Get()
        //{
        //    return null;
        //}

        // GET /products: Listing of products.
        // POST /products: Creating a product.
        // GET /sales: Listing of sales
        // POST /sales: Creating a sale with application of discount rules.
        // DELETE /sales/{id}: cancel the sales id XXX

        // Endpoints should return standardized responses:
        // {
        // "data": [{}],
        // "status": "success",
        // "message": "Operação concluída com sucesso"
        // }

        [HttpPost(Name = "sales")]
        public async Task<IActionResult> CreateSale([FromBody] CreateSaleCommand command)
        {
            var saleDto = await _mediator.Send(command);
            return Ok(new
            {
                data = saleDto,
                status = "success",
                message = "Venda criada com sucesso"
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetSales()
        {
            var result = await _mediator.Send(new GetSalesQuery());
            return Ok(new
            {
                data = result,
                status = "success",
                message = "Lista de vendas obtida com sucesso"
            });
        }
    }
}
