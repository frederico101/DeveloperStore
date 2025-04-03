using System.ComponentModel.DataAnnotations;
using System.Net.Security;

namespace Domain;

 public class Sale
 {
    public Guid SaleId { get; private set; }
    public string SaleNumber { get; private set; }
    public DateTime SaleDate { get; private set; }
    public string Customer { get; private set; }
    public string Branch { get; private set; }
    public List<SaleItem> Items { get; private set; }
    public decimal TotalAmount { get; private set; }
    public bool IsCancelled { get; private set; }
    
    public Sale()
    {
        Items = new List<SaleItem>();
    }
    public Sale(string saleNumber, string customer, string branch, List<SaleItem> items)
    { 
        if (items.Any(item => item.Quantity > 20))
        {
            throw new InvalidOperationException("It's not possible to sell more than 20 identical items.");
        }
        SaleId = Guid.NewGuid();
        SaleNumber = saleNumber;
        SaleDate = DateTime.UtcNow;
        Customer = customer;
        Branch = branch;
        Items = items;
        IsCancelled = false;
        ApplyDiscount();
    }

    public void Cancel()
    {
        IsCancelled = true;
    }

    private void CalculateTotalAmount(decimal? discount = null)
    {
            decimal discountValue = discount ?? 0.0m;
            TotalAmount = Items.Sum(item => item.TotalAmount * (1 - discountValue));
    }

    private void ApplyDiscount()
    {
        foreach (var item in Items)
        {
            if (item.Quantity <= 4)
            {
                CalculateTotalAmount(); // No Apply discount
            } 
            else if (item.Quantity > 4 && item.Quantity < 10)
            {
                CalculateTotalAmount(0.10m); // Apply 10% discount
            }
            else if (item.Quantity >= 10 && item.Quantity <= 20)
            {
                CalculateTotalAmount(0.20m); // Apply 20% discount
            }
            
        }
    }
}
