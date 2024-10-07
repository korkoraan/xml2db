using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace xml2db.DB;

public sealed class ShopContext : DbContext
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Price> Prices { get; set; }
    
    private string _dbPath;

    public ShopContext(string dbPath)
    {
        _dbPath = dbPath;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={_dbPath}");
#if DEBUG
        options.LogTo(Console.WriteLine, LogLevel.Information);
#endif        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Order>()
            .Property(order => order.State)
            .HasConversion<string>();
    }

    private List<ProductBatch> SquashProductBatches(List<ProductBatch> source)
    {
        var result = source
            .OrderBy(b => b.Price.Product.Name)
            .ThenBy(b => b.Price.Value)
            .ToList();
        for (int i = 0; i < result.Count() - 1; ++i)
        {
            while (i < result.Count() - 1
                 && result[i].Price.Product.Name.Equals(result[i + 1].Price.Product.Name)
                 && result[i].Price.Value == result[i + 1].Price.Value)
            {
                result[i].Quantity += result[i + 1].Quantity;
                result.RemoveAt(i + 1);
            }
        }

        return result;
    }
    
    public void AddOrder(Order newOrder)
    {
        if (!newOrder.IsValid())
        {
            Log.Info($"Order is invalid: no={newOrder.No}");
            return;
        }
        
        var order = GetOrder(newOrder);
        if (order != newOrder) // order with the that No exists in DB 
        {
            order.State = OrderState.Deprecated;
            SaveChanges();
            Log.Info($"Order (no=#{order.No}) state set to Deprecated");
        }
        
        newOrder.State = OrderState.Completed;
        newOrder.User = GetUser(newOrder.User);
        newOrder.ProductBatches = SquashProductBatches(newOrder.ProductBatches);
        foreach (var batch in newOrder.ProductBatches)
        {
            var product = GetProduct(batch.Price.Product);
            batch.Price.AddedAt = newOrder.RegDate;
            batch.Price = GetPrice(batch.Price);
            
            batch.Price.Product = product;
            batch.Order = newOrder;
            // TODO: 
        }
        Orders.Add(newOrder);
        SaveChanges();

        Log.Info($@"Order {newOrder} was added");
    }

    private Order GetOrder(Order newOrder)
    {
        var existing = Orders
            .Where(order => order.No == newOrder.No && order.State != OrderState.Deprecated)
            .Take(2)
            .ToList();
        if (existing.Count > 1)
            throw new ProgramException($"multiple not deprecated Orders with No {newOrder.No}");

        var result = existing.FirstOrDefault(newOrder);
        if (result != newOrder && result.State != OrderState.Completed)
            throw new ProgramException($"Found Order with state {result.State}. Editing is not allowed");
        return result;
    }

    private User GetUser(User newUser)
    {
        var existing = Users
            .Where(user => user.Email == newUser.Email)
            .Take(2)
            .ToList();
        if (existing.Count > 1)
            throw new ProgramException($"multiple Users with Email {newUser.Email}");
        var result = existing.FirstOrDefault(newUser);
        if (result != newUser && !result.Fio.Equals(newUser.Fio))
            throw new ProgramException($"user '{result.Fio}' with Email {newUser.Email} exists but Fio's is different: '{newUser.Fio}'");
        return result;
    }

    private Product GetProduct(Product newProduct)
    {
        var existing = Products
            .Where(product => product.Name == newProduct.Name)
            .Take(2)
            .ToList();
        if (existing.Count > 1)
            throw new ProgramException($"multiple Products with Name {newProduct.Name}");
        
        return existing.FirstOrDefault(newProduct);
    }

    /**
     * Подбираем подходящий ценник товара во временной точке совершения покупки.
     * Если находим, то привязываемся к найденному. Если ближайший позднее по времени ценник имеет ту же цену,
     * то делаем вывод, что эта цена была установлена уже на момент текущей покупки - тогда сдвигаем этот ценник
     * по времени в точку данной покупки и ссылаемся на него.
     */
    private Price GetPrice(Price newPrice)
    {
        var result = Prices.FirstOrDefault(price => price.AddedAt == newPrice.AddedAt && price.Value == newPrice.Value);
        if (result is not null)
            return result;

        result = Prices
            .Where(price => price.AddedAt < newPrice.AddedAt)
            .OrderByDescending(price => price.AddedAt)
            .FirstOrDefault();
        
        if (result is not null && result.Value == newPrice.Value)
            return result;
        
        result = Prices
            .Where(price => price.AddedAt > newPrice.AddedAt)
            .OrderBy(price => price.AddedAt)
            .FirstOrDefault();
        // if we have the Price with that value we move it into past
        if (result is not null && result.Value == newPrice.Value)
        {
            Log.Info($"Price {result.Value} moved from {result.AddedAt} to {newPrice.AddedAt}");
            result.AddedAt = newPrice.AddedAt;
            return result;
        }

        return newPrice;
    }
} 
