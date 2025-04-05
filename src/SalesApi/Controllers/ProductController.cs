using MediatR;
using Microsoft.AspNetCore.Mvc;
using SalesApi.Application.Commands;
using SalesApi.Application.Queries;

namespace SalesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost(Name = "create-product")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new
            {
                data = result,
                status = "success",
                message = "Produto criado com sucesso"
            });
        }

        [HttpGet(Name = "get-products")]
        public async Task<IActionResult> GetProducts()
        {
            var result = await _mediator.Send(new GetProductsQuery());
            return Ok(new
            {
                data = result,
                status = "success",
                message = "Lista de produtos obtida com sucesso"
            });
        }
    }
}