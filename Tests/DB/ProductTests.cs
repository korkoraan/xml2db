using xml2db;
using xml2db.DB;
using Xunit;

namespace Tests.DB;

public class ProductTests
{
    [Theory]
    [InlineData("Xiomi 12X  ")]
    [InlineData(" Noname \t 222")]
    [InlineData("c# 9.0")]
    [InlineData("\nxUnit\n")]
    public void IsValid_WithNonEmptyAfterTrimName_ShouldBeOk(string rawName)
    {
        var product = new Product { Name = rawName};
        Assert.True(product.IsValid());
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(" \t \n ")]
    public void IsValid_WithEmptyAfterTrimName_ShouldBeFalse(string rawName)
    {
        var product = new Product { Name = rawName };
        Assert.False(product.IsValid());
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(" \t \n ")]
    [InlineData("Xiomi 12X  ")]
    [InlineData(" Noname \t 222")]
    [InlineData("c# 9.0")]
    [InlineData("\nxUnit\n")]
    public void IsSameAs_WithSameName_ShouldBeSame(string rawName)
    {
        var product1 = new Product { Name = rawName };
        var product2 = new Product { Name = rawName };
        Assert.True(product1.IsSameAs(product2));
    }

    [Theory]
    [InlineData("", "1")]
    [InlineData("\nxUnit\n", "")]
    [InlineData("abc", "bca")]
    public void IsSameAs_WithDifferentNames_ShouldBeDifferent(string rawName1, string rawName2)
    {
        var product1 = new Product { Name = rawName1 };
        var product2 = new Product { Name = rawName2 };
        Assert.False(product1.IsSameAs(product2));
    }
}