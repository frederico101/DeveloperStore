using Bogus;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSubstitute;
using SalesApi.Application.Commands;
using SalesApi.Application.DTOs;
using SalesApi.Application.Queries;
using SalesApi.Controllers;

namespace SalesApi.Tests.Controllers
{
    public class SalesControllerTests
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SalesController> _logger;
        private readonly SalesController _controller;
        private readonly Faker _faker;

        public SalesControllerTests()
        {
            _mediator = Substitute.For<IMediator>();
            _logger = Substitute.For<ILogger<SalesController>>();
            _controller = new SalesController(_logger, _mediator);
            _faker = new Faker();
        }

        private class ApiSuccessResponse<T>
        {
            public T Data { get; set; }
            public string Status { get; set; }
            public string Message { get; set; }
        }

        private class ApiErrorResponse
        {
            public string Type { get; set; }
            public string Error { get; set; }
            public string Detail { get; set; }
            public string Status { get; set; }
            public string Message { get; set; }
        }

        private CreateSaleCommand CreateValidSaleCommand(int itemCount = 3)
        {
            var itemFaker = new Faker<SaleItemDto>()
                .RuleFor(i => i.ProductId, f => Guid.NewGuid())
                .RuleFor(i => i.Quantity, f => f.Random.Int(1, 20))
                .RuleFor(i => i.UnitPrice, f => f.Random.Decimal(10, 1000))
                .RuleFor(i => i.Discount, 0);

            return new CreateSaleCommand(
                saleNumber: _faker.Random.AlphaNumeric(10),
                customerId: Guid.NewGuid().ToString(),
                branchId: Guid.NewGuid().ToString(),
                items: itemFaker.Generate(itemCount)
            );
        }

        private SaleDto CreateSaleDtoFromCommand(CreateSaleCommand command)
        {
            return new SaleDto
            {
                Id = Guid.NewGuid(),
                SaleNumber = command.SaleNumber,
                Date = DateTime.UtcNow,
                CustomerId = Guid.Parse(command.CustomerId),
                BranchId = Guid.Parse(command.BranchId),
                TotalAmount = command.Items.Sum(i => i.UnitPrice * i.Quantity),
                Cancelled = false,
                Items = command.Items.Select(i => new SaleItemDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    Discount = i.Discount
                }).ToList()
            };
        }

        [Fact]
        public async Task CreateSale_ReturnsSuccess_WhenItemsAreValid()
        {
            // Arrange
            var command = CreateValidSaleCommand();
            var saleDto = CreateSaleDtoFromCommand(command);
            _mediator.Send(command).Returns(saleDto);

            // Act
            var result = await _controller.CreateSale(command);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseJson = JsonConvert.SerializeObject(okResult.Value);
            var response = JsonConvert.DeserializeObject<ApiSuccessResponse<SaleDto>>(responseJson);

            Assert.Equal("success", response.Status);
            Assert.Equal("Venda criada com sucesso", response.Message);
            Assert.Equal(saleDto.Id, response.Data.Id);
        }

        [Fact]
        public async Task CreateSale_ReturnsBadRequest_WhenItemQuantityExceeds20()
        {
            // Arrange
            var command = CreateValidSaleCommand();
            var saleDto = CreateSaleDtoFromCommand(command);

            // Add an invalid item
            saleDto.Items.Add(new SaleItemDto
            {
                ProductId = Guid.NewGuid(),
                Quantity = 21,
                UnitPrice = 100,
                Discount = 0
            });

            _mediator.Send(command).Returns(saleDto);

            // Act
            var result = await _controller.CreateSale(command);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var responseJson = JsonConvert.SerializeObject(badRequestResult.Value);
            var response = JsonConvert.DeserializeObject<ApiErrorResponse>(responseJson);

            Assert.Equal("BadRequest", response.Type);
            Assert.Equal("Invalid Sell", response.Error);
            Assert.Equal("You cannot buy more than 20 pieces of the same item", response.Detail);
        }

        [Fact]
        public async Task CreateSale_CallsMediatorWithCorrectCommand()
        {
            // Arrange
            var command = CreateValidSaleCommand();
            var saleDto = CreateSaleDtoFromCommand(command);
            _mediator.Send(command).Returns(saleDto);

            // Act
            await _controller.CreateSale(command);

            // Assert
            await _mediator.Received(1).Send(command);
        }

        [Fact]
        public async Task GetSales_ReturnsSuccess_WithSalesList()
        {
            // Arrange
            var sales = new List<SaleDto>
            {
                CreateSaleDtoFromCommand(CreateValidSaleCommand()),
                CreateSaleDtoFromCommand(CreateValidSaleCommand())
            };

            _mediator.Send(Arg.Any<GetSalesQuery>()).Returns(sales);

            // Act
            var result = await _controller.GetSales();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseJson = JsonConvert.SerializeObject(okResult.Value);
            var response = JsonConvert.DeserializeObject<ApiSuccessResponse<List<SaleDto>>>(responseJson);

            Assert.Equal("success", response.Status);
            Assert.Equal("Lista de vendas obtida com sucesso", response.Message);
            Assert.Equal(2, response.Data.Count);
        }

        [Fact]
        public async Task GetSales_CallsMediatorWithCorrectQuery()
        {
            // Arrange
            _mediator.Send(Arg.Any<GetSalesQuery>()).Returns(new List<SaleDto>());

            // Act
            await _controller.GetSales();

            // Assert
            await _mediator.Received(1).Send(Arg.Any<GetSalesQuery>());
        }

        [Fact]
        public async Task CancelSale_ReturnsSuccess_WhenSaleExists()
        {
            // Arrange
            var saleId = Guid.NewGuid();

            // Act
            var result = await _controller.CancelSale(saleId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseJson = JsonConvert.SerializeObject(okResult.Value);
            var response = JsonConvert.DeserializeObject<ApiSuccessResponse<object>>(responseJson);

            Assert.Equal("success", response.Status);
            Assert.Equal("Venda cancelada com sucesso", response.Message);

            await _mediator.Received(1).Send(Arg.Is<CancelSaleCommand>(x => x.SaleId == saleId));
        }

        [Fact]
        public async Task CancelSale_ReturnsNotFound_WhenSaleDoesNotExist()
        {
            // Arrange
            var saleId = Guid.NewGuid();
            var errorMessage = "Sale not found";

            _mediator.When(x => x.Send(Arg.Is<CancelSaleCommand>(c => c.SaleId == saleId)))
                     .Throw(new Exception(errorMessage));

            // Act
            var result = await _controller.CancelSale(saleId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var responseJson = JsonConvert.SerializeObject(notFoundResult.Value);
            var response = JsonConvert.DeserializeObject<ApiErrorResponse>(responseJson);

            Assert.Equal("error", response.Status);
            Assert.Equal(errorMessage, response.Message);

            await _mediator.Received(1).Send(Arg.Is<CancelSaleCommand>(x => x.SaleId == saleId));
        }
    }
}