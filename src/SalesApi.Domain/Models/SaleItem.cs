using Domain;

public class SaleItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SaleId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Discount { get; private set; }
    public decimal TotalAmount { get; private set; }

    public SaleItem(Guid productId, int quantity, decimal unitPrice, decimal discount)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
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