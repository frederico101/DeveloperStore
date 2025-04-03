using Bogus;
using Domain;
using Xunit;

namespace SalesApi.Tests;
public class SaleTests
{
    [Theory]
   // [InlineData(4, 360)]  // 10% discount for 4 items
    [InlineData(5, 450)] // 10% discount for 5 items
    public void Should_Apply_10Percent_Discount_For_5_Or_More_Identical_Items(int quantity, decimal expectedTotalAmount)
    {
         // Arrange
        var faker = new Faker<SaleItem>()
            .CustomInstantiator(f => new SaleItem(Guid.NewGuid(), f.Commerce.ProductName(), quantity, 100, 0));
        var items = faker.Generate(1);

        // Act
        var sale = new Sale("SALE001", "Customer A", "Branch A", items);

        // Assert
        Assert.Equal(expectedTotalAmount, sale.TotalAmount);
    }

    [Theory]
    [InlineData(10, 800)] // 20% discount for 10 items
    [InlineData(15, 1200)] // 20% discount for 15 items
    [InlineData(20, 1600)] // 20% discount for 20 items
    public void Should_Apply_20Percent_Discount_For_10__Or_20_Identical_Items(int quantity, decimal expectedTotalAmount)
    {
        // Arrange
        var faker = new Faker<SaleItem>()
            .CustomInstantiator(f => new SaleItem(Guid.NewGuid(), f.Commerce.ProductName(), quantity, 100, 0));

        var items = faker.Generate(1);

        // Act
        var sale = new Sale("SALE001", "Customer A", "Branch A", items);

        // Assert
        Assert.Equal(expectedTotalAmount, sale.TotalAmount);
    }

    [Fact]
    public void Should_Not_Allow_Sale_Above_20_Identical_Items()
    {
        // Arrange
        var faker = new Faker<SaleItem>()
            .CustomInstantiator(f => new SaleItem(Guid.NewGuid(), f.Commerce.ProductName(), 21, 100, 0));

        var items = faker.Generate(1);

        // Act
        Action sale = () => new Sale("SALE003", "Customer C", "Branch C", items);

        // Assert
        Assert.Throws<InvalidOperationException>(sale);
    }

    [Theory]
    [InlineData(1, 100)]  // 1 item, no discount
    [InlineData(2, 200)]  // 2 items, no discount
    [InlineData(3, 300)]  // 3 items, no discount
    [InlineData(4, 400)]  // 4 items, no discount
    public void Should_Not_Apply_Discount_For_Purchases_Below_4_Items(int quantity, decimal expectedTotalAmount)
    {
        // Arrange
        var faker = new Faker<SaleItem>()
            .CustomInstantiator(f => new SaleItem(Guid.NewGuid(), f.Commerce.ProductName(), quantity, 100, 0)); // $100 per item

        var items = faker.Generate(1);

        // Act
        var sale = new Sale("SALE004", "Customer D", "Branch D", items);

        // Assert
        Assert.Equal(expectedTotalAmount, sale.TotalAmount); // No discount applied
    }
}