public class SaleItem
{
    public string ProductName { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Discount { get; private set; }
    public decimal TotalAmount { get; private set; }

    public SaleItem(string productName, int quantity, decimal unitPrice, decimal discount)
    {
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Discount = discount;
        CalculateTotalPrice();
    }
    //public void ApplyDiscount(decimal discount)
    //{
    //    Discount = discount;
    //    CalculateTotalPrice();
    //}
    private void CalculateTotalPrice()
    {
        TotalAmount = (UnitPrice * Quantity) - (UnitPrice * Quantity * Discount);
    }
}