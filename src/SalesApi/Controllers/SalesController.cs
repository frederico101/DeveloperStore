using Microsoft.AspNetCore.Mvc;
using MediatR;
using SalesApi.Application.Commands;
using SalesApi.Application.Queries;
using SalesApi.Application.DTOs;

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
            SaleDto saleDto = await _mediator.Send(command);
            List<SaleItemDto> result = saleDto.Items.Where(x => x.Quantity > 20).ToList();
            var test = result.Where(a => a.Quantity > 20);   
            if (test.Any( a => a.Quantity > 20))

                   return BadRequest(new
                   {
                   Type = "BadRequest",
                   Error = "Invalid Sell",
                   Detail = "You cannot buy more than 20 pieces of the same item"
                   });

            return Ok(new
            {
                data = saleDto,
                status = "success",
                message = "Sale created successfully"
            });
        }

        [HttpGet(Name = "get-sales")]
        public async Task<IActionResult> GetSales()
        {
            var result = await _mediator.Send(new GetSalesQuery());
            return Ok(new
            {
                data = result,
                status = "success",
                message = "List of sales obtained successfully"
            });
        }

        [HttpDelete("{id}", Name = "cancel-sale")]
        public async Task<IActionResult> CancelSale(Guid id)
        {
            try
            {
                
                await _mediator.Send(new CancelSaleCommand { SaleId = id });

                return Ok(new
                {
                    status = "success",
                    message = "sale canceled with success"
                });
            }
            catch (Exception ex)
            {
               
                return NotFound(new
                {
                    status = "error",
                    message = ex.Message
                });
            }
        }
    }
}
