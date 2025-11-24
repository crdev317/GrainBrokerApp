using Microsoft.EntityFrameworkCore;

public class GrainBrokerContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Order> GrainOrders { get; set; }

    public GrainBrokerContext(DbContextOptions<GrainBrokerContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Customer
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(e => e.Location).HasMaxLength(200);
        });

        // Supplier
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.Property(e => e.Location).HasMaxLength(200);
        });

        // Order
        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(e => e.CostOfDelivery)
                  .HasColumnType("decimal(18,2)");

            // Relationships
            entity.HasOne(o => o.Customer)
                  .WithMany(c => c.Orders)
                  .HasForeignKey(o => o.CustomerId);

            entity.HasOne(o => o.Supplier)
                  .WithMany(s => s.OrdersFulfilled)
                  .HasForeignKey(o => o.SupplierId);
        });
    }
}
