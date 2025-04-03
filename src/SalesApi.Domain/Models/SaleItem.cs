using Domain;

public class SaleItem
{
    public Guid Id { get; set; } = Guid.NewGuid(); // Unique identifier
    public Guid SaleId { get; set; }  // FK to Sales
    public Guid ProductId { get; set; }
    public string ProductName { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Discount { get; private set; }
    public decimal TotalAmount { get; private set; }

    public SaleItem(Guid productId, string productName, int quantity, decimal unitPrice, decimal discount)
    {
        Id = Guid.NewGuid(); // Ensure a unique Id for each SaleItem
        ProductId = productId; // Use the provided ProductId from the request
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Discount = discount;
        CalculateTotalPrice();
    }
  
    private void CalculateTotalPrice()
    {
        TotalAmount = (UnitPrice * Quantity) - (UnitPrice * Quantity * Discount);
    }
}