using Bogus;
using Domain;
using Xunit;

namespace SalesApi.Tests;
public class SaleTests
{
    [Fact]
    public void Should_Apply_10Percent_Discount_For_4_Or_More_Items()
    {
         // Arrange
        var faker = new Faker<SaleItem>()
            .CustomInstantiator(f => new SaleItem(f.Commerce.ProductName(), 4, 100, 0));

        var items = faker.Generate(1);

        // Act
        var sale = new Sale("SALE001", "Customer A", "Branch A", items);

        // Assert
        Assert.Equal(360, sale.TotalAmount); // 10% discount applied
    }
}