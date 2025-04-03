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
