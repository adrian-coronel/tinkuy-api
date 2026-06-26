using Microsoft.EntityFrameworkCore;
using StockLinkApi.Models;

namespace StockLinkApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Store> Stores => Set<Store>();
    public DbSet<Camera> Cameras => Set<Camera>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Inventory> Inventory => Set<Inventory>();
    public DbSet<Detection> Detections => Set<Detection>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<Store>(e =>
        {
            e.HasKey(s => s.Id);
            e.Property(s => s.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(s => s.Name).HasMaxLength(150).IsRequired();
            e.Property(s => s.Category).HasMaxLength(50);
            e.Property(s => s.Address).HasMaxLength(255);
            e.Property(s => s.Phone).HasMaxLength(20);
            e.Property(s => s.CreatedAt).HasDefaultValueSql("now()");
        });

        model.Entity<Camera>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(c => c.Status).HasMaxLength(20);
            e.Property(c => c.CreatedAt).HasDefaultValueSql("now()");
            e.HasOne(c => c.Store).WithMany(s => s.Cameras).HasForeignKey(c => c.StoreId);
        });

        model.Entity<Product>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(p => p.Name).HasMaxLength(150).IsRequired();
            e.Property(p => p.Category).HasMaxLength(50);
            e.Property(p => p.CreatedAt).HasDefaultValueSql("now()");
        });

        model.Entity<Inventory>(e =>
        {
            e.HasKey(i => i.Id);
            e.Property(i => i.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(i => i.Price).HasColumnType("numeric(10,2)");
            e.Property(i => i.UpdatedAt).HasDefaultValueSql("now()");
            e.HasIndex(i => new { i.StoreId, i.ProductId }).IsUnique();
            e.HasOne(i => i.Store).WithMany(s => s.Inventory).HasForeignKey(i => i.StoreId);
            e.HasOne(i => i.Product).WithMany(p => p.Inventory).HasForeignKey(i => i.ProductId);
        });

        model.Entity<Detection>(e =>
        {
            e.HasKey(d => d.Id);
            e.Property(d => d.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(d => d.RawImageUrl).HasMaxLength(500);
            e.Property(d => d.DetectedProducts).HasColumnType("jsonb");
            e.Property(d => d.CreatedAt).HasDefaultValueSql("now()");
            e.HasOne(d => d.Camera).WithMany(c => c.Detections).HasForeignKey(d => d.CameraId);
        });

        model.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.Property(u => u.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(u => u.Name).HasMaxLength(150);
            e.Property(u => u.Phone).HasMaxLength(20);
            e.Property(u => u.CreatedAt).HasDefaultValueSql("now()");
        });

        model.Entity<Reservation>(e =>
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(r => r.ReservationCode).HasMaxLength(20);
            e.Property(r => r.Status).HasMaxLength(20);
            e.Property(r => r.CreatedAt).HasDefaultValueSql("now()");
            e.HasOne(r => r.User).WithMany(u => u.Reservations).HasForeignKey(r => r.UserId);
            e.HasOne(r => r.Inventory).WithMany(i => i.Reservations).HasForeignKey(r => r.InventoryId);
        });

        model.Entity<Notification>(e =>
        {
            e.HasKey(n => n.Id);
            e.Property(n => n.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(n => n.Type).HasMaxLength(50).IsRequired();
            e.Property(n => n.Title).HasMaxLength(150).IsRequired();
            e.Property(n => n.Message).HasMaxLength(500).IsRequired();
            e.Property(n => n.Payload).HasColumnType("jsonb");
            e.Property(n => n.CreatedAt).HasDefaultValueSql("now()");
            e.HasOne(n => n.User).WithMany().HasForeignKey(n => n.UserId).IsRequired(false);
        });
    }
}
