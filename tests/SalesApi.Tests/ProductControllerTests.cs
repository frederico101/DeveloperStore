using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NSubstitute;
using SalesApi.Application.Commands;
using SalesApi.Application.DTOs;
using SalesApi.Application.Queries;
using SalesApi.Controllers;
using Xunit;

namespace SalesApi.Tests.Controllers
{
    public class ProductControllerTests
    {
        private readonly IMediator _mediator;
        private readonly ProductController _controller;
        private readonly Faker<ProductDto> _productFaker;
        private readonly Faker<CreateProductCommand> _productCommandFaker;

        public ProductControllerTests()
        {
            _mediator = Substitute.For<IMediator>();
            _controller = new ProductController(_mediator);

            // Configure Bogus fakers
            _productFaker = new Faker<ProductDto>()
                .RuleFor(p => p.Id, f => Guid.NewGuid())
                .RuleFor(p => p.Title, f => f.Commerce.ProductName())
                .RuleFor(p => p.Price, f => f.Random.Decimal(1, 1000))
                .RuleFor(p => p.Description, f => f.Lorem.Sentence())
                .RuleFor(p => p.Category, f => f.Commerce.Categories(1)[0])
                .RuleFor(p => p.Image, f => f.Image.PicsumUrl());

            _productCommandFaker = new Faker<CreateProductCommand>()
                .RuleFor(p => p.Title, f => f.Commerce.ProductName())
                .RuleFor(p => p.Price, f => f.Random.Decimal(1, 1000))
                .RuleFor(p => p.Description, f => f.Lorem.Sentence())
                .RuleFor(p => p.Category, f => f.Commerce.Categories(1)[0])
                .RuleFor(p => p.Image, f => f.Image.PicsumUrl());
        }

        private class ApiResponse<T>
        {
            public T Data { get; set; }
            public string Status { get; set; }
            public string Message { get; set; }
        }

        [Fact]
        public async Task CreateProduct_ReturnsSuccessResponse_WhenCommandIsValid()
        {
            // Arrange
            var expectedProduct = _productFaker.Generate();
            var command = new CreateProductCommand
            {
                Title = expectedProduct.Title,
                Price = expectedProduct.Price,
                Description = expectedProduct.Description,
                Category = expectedProduct.Category,
                Image = expectedProduct.Image
            };

            _mediator.Send(command).Returns(expectedProduct);

            // Act
            var result = await _controller.CreateProduct(command);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseJson = JsonConvert.SerializeObject(okResult.Value);
            var response = JsonConvert.DeserializeObject<ApiResponse<ProductDto>>(responseJson);

            Assert.Equal("success", response.Status);
            Assert.Equal("Produto criado com sucesso", response.Message);

            // Validate product data
            Assert.Equal(expectedProduct.Id, response.Data.Id);
            Assert.Equal(command.Title, response.Data.Title);
            Assert.Equal(command.Price, response.Data.Price);
            Assert.Equal(command.Description, response.Data.Description);
            Assert.Equal(command.Category, response.Data.Category);
            Assert.Equal(command.Image, response.Data.Image);

            // Additional validations
            Assert.NotEqual(Guid.Empty, response.Data.Id);
            Assert.NotNull(response.Data.Title);
            Assert.True(response.Data.Price > 0);
        }

        [Fact]
        public async Task GetProducts_ReturnsSuccessResponse_WithProductList()
        {
            // Arrange
            var expectedProducts = _productFaker.Generate(3);
            _mediator.Send(Arg.Any<GetProductsQuery>()).Returns(expectedProducts);

            // Act
            var result = await _controller.GetProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseJson = JsonConvert.SerializeObject(okResult.Value);
            var response = JsonConvert.DeserializeObject<ApiResponse<List<ProductDto>>>(responseJson);

            Assert.Equal("success", response.Status);
            Assert.Equal("Lista de produtos obtida com sucesso", response.Message);
            Assert.Equal(3, response.Data.Count);

            // Validate each product in the list
            foreach (var product in response.Data)
            {
                Assert.NotEqual(Guid.Empty, product.Id);
                Assert.NotNull(product.Title);
                Assert.True(product.Price > 0);
            }
        }

        [Fact]
        public async Task CreateProduct_CallsMediatorWithCorrectCommand()
        {
            // Arrange
            var command = _productCommandFaker.Generate();
            _mediator.Send(Arg.Any<CreateProductCommand>()).Returns(_productFaker.Generate());

            // Act
            await _controller.CreateProduct(command);

            // Assert
            await _mediator.Received(1).Send(command);
        }

        [Fact]
        public async Task GetProducts_CallsMediatorWithCorrectQuery()
        {
            // Arrange
            _mediator.Send(Arg.Any<GetProductsQuery>()).Returns(new List<ProductDto>());

            // Act
            await _controller.GetProducts();

            // Assert
            await _mediator.Received(1).Send(Arg.Is<GetProductsQuery>(x => x != null));
        }
    }
}