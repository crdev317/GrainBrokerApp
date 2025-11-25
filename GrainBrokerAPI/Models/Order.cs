public class Order
{
    public Guid Id { get; set; }

    public TimeSpan OrderDate { get; set; }
    public Guid PurchaseOrder { get; set; }
    // FKs
    public Guid CustomerId { get; set; }
    public Guid SupplierId { get; set; }

    public Customer? Customer { get; set; }
    public Supplier? Supplier { get; set; }

    public int OrderReqAmtTon { get; set; }
    public int SuppliedAmtTon { get; set; }

    public decimal CostOfDelivery { get; set; }
}
