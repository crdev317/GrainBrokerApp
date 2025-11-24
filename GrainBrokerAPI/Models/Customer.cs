public class Customer
{
    public Guid Id { get; set; }
    public string Location { get; set; }

    // Navigation
    public ICollection<Order> Orders { get; set; }
}