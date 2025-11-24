public class Supplier
{
    public Guid Id { get; set; }
    public string Location { get; set; }

    // Navigation
    public ICollection<Order> OrdersFulfilled { get; set; }
}