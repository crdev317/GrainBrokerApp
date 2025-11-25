public class Supplier
{
    public Guid Id { get; set; }
    public string Location { get; set; }
    public ICollection<Order>? OrdersFulfilled { get; set; }
}