using xml2db.DB;
using Xunit;
using Assert = Xunit.Assert;

namespace Tests.DB;

public class PriceTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(313)]
    [InlineData(3461)]
    [InlineData(188)]
    [InlineData(int.MaxValue)]
    public void IsValid_WithFinitePositiveValue_ShouldBeOk(int value)
    {
        var product = new Product { Name = "super product" };
        var time = DateTime.Parse("03/04/4018 04:22:16");
        var price = new Price { Value = value, AddedAt = time, Product = product };
        Assert.True(price.IsValid());
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-361)]
    [InlineData(-255188)]
    [InlineData(int.MinValue)]
    public void IsValid_WithNonPositiveValue_ShouldBeFalse(int value)
    {
        var product = new Product { Name = "super product" };
        var time = DateTime.Parse("03/04/4018 04:22:16");
        var price = new Price { Value = value, AddedAt = time, Product = product };
        Assert.False(price.IsValid());
    }

    [Theory]
    [InlineData("screwdriver")]
    public void IsValid_WithValidProduct_ShouldBeOk(string productName)
    {
        var product = new Product { Name = productName };
        var time = DateTime.Parse("03/04/4018 04:22:16");
        var price = new Price { Value = 1, AddedAt = time, Product = product };
        Assert.True(price.IsValid());
    }

    [Theory]
    [InlineData(" \t \n ")]
    public void IsValid_WithInvalidProduct_ShouldBeFalse(string productName)
    {
        var product = new Product { Name = productName };
        var time = DateTime.Parse("03/04/4018 04:22:16");
        var price = new Price { Value = 1, AddedAt = time, Product = product };
        Assert.False(price.IsValid());
    }

    [Theory]
    [InlineData(12, "03/04/4018 04:22:16","pliers")]
    [InlineData(-12, "03/04/3018 04:22:16"," pliers ")]
    public void IsSameAs_WithSameValuesAddedAtProduct_ShouldBeSame(int value, string time, string productName)
    {
        var product1 = new Product { Name = productName };
        var product2 = new Product { Name = productName };
        var price1 = new Price { Value = value, AddedAt = DateTime.Parse(time), Product = product1 };
        var price2 = new Price { Value = value, AddedAt = DateTime.Parse(time), Product = product2 };
        Assert.True(price1.IsSameAs(price2));
    }

    [Theory]
    [InlineData(12, 11, "03/04/4018 04:22:16","pliers")]
    [InlineData(-12, 0, "03/04/3018 04:22:16"," pliers ")]
    public void IsSameAs_WithDifferentValues_ShouldBeDifferent(int value1, int value2, string time, string productName)
    {
        var product1 = new Product { Name = productName };
        var product2 = new Product { Name = productName };
        var price1 = new Price { Value = value1, AddedAt = DateTime.Parse(time), Product = product1 };
        var price2 = new Price { Value = value2, AddedAt = DateTime.Parse(time), Product = product2 };
        Assert.False(price1.IsSameAs(price2));
    }
    
    [Theory]
    [InlineData(12, "03/04/4018 04:22:16", "01/01/1111","pliers")]
    public void IsSameAs_WithDifferentAddedAt_ShouldBeDifferent(int value, string time1, string time2, string productName)
    {
        var product1 = new Product { Name = productName };
        var product2 = new Product { Name = productName };
        var price1 = new Price { Value = value, AddedAt = DateTime.Parse(time1), Product = product1 };
        var price2 = new Price { Value = value, AddedAt = DateTime.Parse(time2), Product = product2 };
        Assert.False(price1.IsSameAs(price2));
    }

    [Theory]
    [InlineData(12, "03/04/4018 04:22:16", "01/01/1111","pliers")]
    public void IsSameAs_WithDifferentProduct_ShouldBeDifferent(int value, string time, string productName1, string productName2)
    {
        var product1 = new Product { Name = productName1 };
        var product2 = new Product { Name = productName2 };
        var price1 = new Price { Value = value, AddedAt = DateTime.Parse(time), Product = product1 };
        var price2 = new Price { Value = value, AddedAt = DateTime.Parse(time), Product = product2 };
        Assert.False(price1.IsSameAs(price2));
    }
    
}