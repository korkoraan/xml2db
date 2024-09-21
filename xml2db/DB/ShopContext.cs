using Microsoft.EntityFrameworkCore;

namespace xml2db.DB;

public sealed class ShopContext : DbContext
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<PricedProduct> PricedProducts { get; set; }
    
    private string _dbPath;

    public ShopContext()
    {
        _dbPath = Path.Join("shop.db");
    }
    
    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={_dbPath}");
} 
