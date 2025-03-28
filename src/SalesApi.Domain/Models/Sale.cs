namespace Domain
{
    public class Sale
{
    public Guid Id { get; private set; }
    public string SaleNumber { get; private set; }
    public DateTime SaleDate { get; private set; }
    public string Customer { get; private set; }
    public string Branch { get; private set; }
    public List<SaleItem> Items { get; private set; }
    public decimal TotalAmount { get; private set; }
    public bool IsCancelled { get; private set; }

    public Sale(string saleNumber, string customer, string branch, List<SaleItem> items)
    {
        Id = Guid.NewGuid();
        SaleNumber = saleNumber;
        SaleDate = DateTime.UtcNow;
        Customer = customer;
        Branch = branch;
        Items = items;
        TotalAmount = CalculateTotalAmount();
        IsCancelled = false;
    }

    public void Cancel()
    {
        IsCancelled = true;
    }

    private decimal CalculateTotalAmount()
    {
        return Items.Sum(item => item.TotalAmount);
    }
 }
}
