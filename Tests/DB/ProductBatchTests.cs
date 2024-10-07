using xml2db.DB;
using Xunit;

namespace Tests;

public class ProductBatchTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(123)]
    [InlineData(int.MaxValue)]
    public void IsValid_WithPositiveQuantity_ShouldBeOk(int quantity)
    {
        var order = new Order { User = new User() };
        var price = new Price { Value = 1, 
            AddedAt = DateTime.Parse("01/04/18 05:26:36"), 
            Product = new Product { Name = "mouse" }
        };
        var batch = new ProductBatch { Order = order, Price = price, Quantity = quantity };
        Assert.True(batch.IsValid());
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-12344)]
    [InlineData(int.MinValue)]
    public void IsValid_WithNonPositiveQuantity_ShouldBeFalse(int quantity)
    {
        var order = new Order { User = new User() };
        var price = new Price { Value = 1, 
            AddedAt = DateTime.Parse("01/04/18 05:26:36"), 
            Product = new Product { Name = "mouse" }
        };
        var batch = new ProductBatch { Order = order, Price = price, Quantity = quantity };
        Assert.False(batch.IsValid());
    }

    [Theory]
    [InlineData(1, "nails")]
    [InlineData(2, "fire")]
    [InlineData(4, "water")]
    [InlineData(int.MaxValue, "\t dfgdfg\n \\")]
    public void IsValid_WithValidPrice_ShouldBeOk(int priceValue, string productName) 
    {
        var order = new Order { User = new User() };
        var price = new Price { Value = priceValue, 
            AddedAt = DateTime.Parse("01/04/18 05:26:36"), 
            Product = new Product { Name = productName }
        };
        var batch = new ProductBatch { Order = order, Price = price, Quantity = 1 };
        Assert.True(batch.IsValid());
    }
    
    [Theory]
    [InlineData(0, "nails")]
    [InlineData(-1, "fire")]
    [InlineData(4, "  ")]
    [InlineData(int.MinValue, "\t dfgdfg\n \\")]
    public void IsValid_WithInvalidPrice_ShouldBeFalse(int priceValue, string productName) 
    {
        var order = new Order { User = new User() };
        var price = new Price { Value = priceValue, 
            AddedAt = DateTime.Parse("01/04/18 05:26:36"), 
            Product = new Product { Name = productName }
        };
        var batch = new ProductBatch { Order = order, Price = price, Quantity = 1 };
        Assert.False(batch.IsValid());
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    [InlineData(-123)]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    public void IsSameAs_WithSameQuantity_ShouldBeSame(int quantity)
    {
        var priceValue = 1;
        var priceAddedAt = DateTime.Parse("01/04/18 05:26:36");
        var productName = "paper";
        var order1 = new Order { User = new User() };
        var price1 = new Price { Value = priceValue, AddedAt = priceAddedAt, Product = new Product { Name = productName }};
        var batch1 = new ProductBatch { Order = order1, Price = price1, Quantity = quantity };
        var order2 = new Order { User = new User() };
        var price2 = new Price { Value = priceValue, AddedAt = priceAddedAt, Product = new Product { Name = productName }};
        var batch2 = new ProductBatch { Order = order2, Price = price2, Quantity = quantity };
        Assert.True(batch1.IsSameAs(batch2));
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(0, 213)]
    [InlineData(-132, -212)]
    public void IsSameAs_WithDifferentQuantity_ShouldBeDifferent(int quantity1, int quantity2)
    {
        var priceValue = 1;
        var priceAddedAt = DateTime.Parse("01/04/18 05:26:36");
        var productName = "paper";
        var order1 = new Order { User = new User() };
        var price1 = new Price { Value = priceValue, AddedAt = priceAddedAt, Product = new Product { Name = productName }};
        var batch1 = new ProductBatch { Order = order1, Price = price1, Quantity = quantity1 };
        var order2 = new Order { User = new User() };
        var price2 = new Price { Value = priceValue, AddedAt = priceAddedAt, Product = new Product { Name = productName }};
        var batch2 = new ProductBatch { Order = order2, Price = price2, Quantity = quantity2 };
        Assert.False(batch1.IsSameAs(batch2));
    }
    
    [Theory]
    [InlineData(0, "01/04/18 05:26:36", "keyboard")]
    [InlineData(-244, "01/04/18 05:26:36", "keyboard")]
    [InlineData(554, "01/04/18", "")]
    [InlineData(0, "01/04/18 05:26:36", "\t")]
    public void IsSameAs_WithSamePrice_ShouldBeSame(int priceValue, string priceAddedAt, string productName)
    {
        var quantity = 1;
        var order1 = new Order { User = new User() };
        var price1 = new Price { Value = priceValue, AddedAt = DateTime.Parse(priceAddedAt), Product = new Product { Name = productName }};
        var batch1 = new ProductBatch { Order = order1, Price = price1, Quantity = quantity };
        var order2 = new Order { User = new User() };
        var price2 = new Price { Value = priceValue, AddedAt = DateTime.Parse(priceAddedAt), Product = new Product { Name = productName }};
        var batch2 = new ProductBatch { Order = order2, Price = price2, Quantity = quantity };
        Assert.True(batch1.IsSameAs(batch2));
    }

    [Theory]
    [InlineData(0, "01/04/18 05:26:36", "keyboard")]
    [InlineData(-244, "01/04/18 05:26:36", "keyboard")]
    [InlineData(554, "01/04/18", "")]
    [InlineData(0, "01/04/18 05:26:36", "\t")]
    public void IsSameAs_WithDifferentPrice_ShouldBeDifferent(int priceValue, string priceAddedAt, string productName)
    {
        var quantity = 1;
        var order1 = new Order { User = new User() };
        var price1 = new Price { Value = priceValue, AddedAt = DateTime.Parse(priceAddedAt), Product = new Product { Name = productName }};
        var batch1 = new ProductBatch { Order = order1, Price = price1, Quantity = quantity };
        var order2 = new Order { User = new User() };
        var price2 = new Price { Value = priceValue + 1, AddedAt = DateTime.Parse(priceAddedAt), Product = new Product { Name = productName }};
        var batch2 = new ProductBatch { Order = order2, Price = price2, Quantity = quantity };
        Assert.False(batch1.IsSameAs(batch2));
    }
    
}