using Microsoft.EntityFrameworkCore.ChangeTracking;
using xml2db.DB;
using Xunit;

namespace Tests.DB;

public class OrderTests
{

    [Theory]
    [InlineData(1)]
    [InlineData(313)]
    [InlineData(3461)]
    [InlineData(188)]
    [InlineData(int.MaxValue)]
    public void IsValid_WithPositiveNo_ShouldBeOk(int no)
    {
        var regDate = DateTime.Parse("01/04/2018");
        var user = new User { Fio = "Ethan Coen", Email = "ethan@coen.sup" };
        var order = new Order { No = no, RegDate = regDate, User = user };
        Assert.True(order.IsValid());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-313)]
    [InlineData(-3461)]
    [InlineData(-188)]
    [InlineData(int.MinValue)]
    public void IsValid_WithNonPositiveNo_ShouldBeFalse(int no)
    {
        var regDate = DateTime.Parse("01/04/2018");
        var user = new User { Fio = "Ethan Coen", Email = "ethan@coen.sup" };
        var order = new Order { No = no, RegDate = regDate, User = user };
        Assert.False(order.IsValid());
    }
    
    [Theory]
    [InlineData("01/04/2018")]
    [InlineData("03/04/4018 04:22:16")]
    [InlineData("01/04/0034")]
    [InlineData("01/01/0034 00:00:00")]
    public void IsValid_WithRegDateAfterCrucification_ShouldBeOk(string regDate)
    {
        var no = 1;
        var user = new User { Fio = "Ethan Coen", Email = "ethan@coen.sup" };
        var order = new Order { No = no, RegDate = DateTime.Parse(regDate), User = user };
        Assert.True(order.IsValid());
    }

    [Theory]
    [InlineData("01/04/0018")]
    [InlineData("03/04/0032 04:22:16")]
    [InlineData("01/04/0003")]
    [InlineData("01/01/0033 00:00:00")]
    public void IsValid_WithRegDateBeforeCrucification_ShouldBeFalse(string regDate)
    {
        var no = 1;
        var user = new User { Fio = "Ethan Coen", Email = "ethan@coen.sup" };
        var order = new Order { No = no, RegDate = DateTime.Parse(regDate), User = user };
        Assert.False(order.IsValid());
    }

    [Theory]
    [InlineData("Joel Coen\n", " joel@CollectionEntry.sup")]
    public void IsValid_WithValidUser_ShouldBeOk(string userFio, string userEmail)
    {
        var no = 1;
        var regDate = DateTime.Parse("03/04/4018 04:22:16");
        var user = new User { Fio = userFio, Email = userEmail };
        var order = new Order { No = no, RegDate = regDate, User = user };
        Assert.True(order.IsValid());
    }
    
    [Theory]
    [InlineData("\n", " joel@CollectionEntry.sup")]
    [InlineData("\tJoel Coen\n", "               ")]
    [InlineData("Joel Coen", " .sup")]
    public void IsValid_WithInvalidUser_ShouldBeFalse(string userFio, string userEmail)
    {
        var no = 1;
        var regDate = DateTime.Parse("03/04/4018 04:22:16");
        var user = new User { Fio = userFio, Email = userEmail };
        var order = new Order { No = no, RegDate = regDate, User = user };
        Assert.False(order.IsValid());
    }

    [Theory]
    [InlineData(321, new[] { 1, 2, 3 }, new[] { 1, 10, 100 })]
    [InlineData(60, new[] { 10, 20, 30 }, new[] { 1, 1, 1 })]
    public void IsValid_WithValidBatches_ShouldBeOk(int sum, int[] values, int[] quantities)
    {
        var no = 1;
        var regDate = DateTime.Parse("03/04/4018 04:22:16");
        var user = new User { Fio = "Joel Coen", Email = " joel@CollectionEntry.sup" };
        var order = new Order { No = no, RegDate = regDate, User = user, Sum = sum };
        for (int i = 0; i < values.Length; ++i)
        {
            var product = new Product { Name = "nothing" + i };
            var price = new Price{ AddedAt = regDate, Value = values[i], Product = product };
            order.ProductBatches.Add(new ProductBatch{ Price = price, Quantity = quantities[i]});
        }
        Assert.True(order.IsValid());
    }
    
    [Theory]
    [InlineData(321, new[] { -1, 2, 3 }, new[] { 1, 10, 100 })]
    [InlineData(60, new[] { 10, 20, 30 }, new[] { 0, 1, 1 })]
    public void IsValid_WithInvalidBatches_ShouldBeFalse(int sum, int[] values, int[] quantities)
    {
        var no = 1;
        var regDate = DateTime.Parse("03/04/4018 04:22:16");
        var user = new User { Fio = "Joel Coen", Email = " joel@CollectionEntry.sup" };
        var order = new Order { No = no, RegDate = regDate, User = user };
        for (int i = 0; i < values.Length; ++i)
        {
            var product = new Product { Name = "nothing" + i };
            var price = new Price{ AddedAt = regDate, Value = values[i], Product = product };
            order.ProductBatches.Add(new ProductBatch{ Price = price, Quantity = quantities[i]});
        }
        Assert.False(order.IsValid());
    }
    
    [Theory]
    [InlineData(321, new[] { 1, 2, 3 }, new[] { 1, 10, 100 })]
    [InlineData(60, new[] { 10, 20, 30 }, new[] { 1, 1, 1 })]
    public void IsValid_WithCorrectSum_ShouldBeOk(int expected, int[] values, int[] quantities)
    {
        var no = 1;
        var regDate = DateTime.Parse("03/04/4018 04:22:16");
        var user = new User { Fio = "Ethan Coen", Email = "ethan@coen.sup" };
        var order = new Order { No = no, RegDate = regDate, User = user, Sum = expected };
        for (int i = 0; i < values.Length; ++i)
        {
            var product = new Product { Name = "nothing" + i };
            var price = new Price{ AddedAt = regDate, Value = values[i], Product = product };
            order.ProductBatches.Add(new ProductBatch{ Price = price, Quantity = quantities[i]});
        }
        Assert.True(order.IsValid());
    }
    
    [Theory]
    [InlineData(-321, new[] { 1, 2, 3 }, new[] { 1, 10, 100 })]
    [InlineData(10, new[] { 1, 2, 3 }, new[] { 0, 0, 0 })]
    public void IsValid_WithIncorrectSum_ShouldBeFalse(int expected, int[] values, int[] quantities)
    {
        var no = 1;
        var regDate = DateTime.Parse("03/04/4018 04:22:16");
        var user = new User { Fio = "Ethan Coen", Email = "ethan@coen.sup" };
        var order = new Order { No = no, RegDate = regDate, User = user, Sum = expected };
        for (int i = 0; i < values.Length; ++i)
        {
            var product = new Product { Name = "nothing" + 1 };
            var price = new Price{ AddedAt = regDate, Value = values[i], Product = product };
            order.ProductBatches.Add(new ProductBatch{ Price = price, Quantity = quantities[i]});
        }
        Assert.False(order.IsValid());
    }

    [Theory]
    [InlineData("Collective Soul", " collect@soul.alt", "Collective     Soul", " collect@soul.alt ")]
    public void IsSameAs_WithSameUser_ShouldBeSame(string user1Fio, string user1Email, string user2Fio, string user2Email)
    {
        var no = 1;
        var regDate = DateTime.Parse("03/04/4018 04:22:16");
        var user1 = new User { Fio = user1Fio, Email = user1Email };
        var user2 = new User { Fio = user2Fio, Email = user2Email };
        var order1 = new Order { No = no, RegDate = regDate, User = user1 };
        var order2 = new Order { No = no, RegDate = regDate, User = user2 };
        Assert.True(order1.IsSameAs(order2));
    }

    [Theory]
    [InlineData("Linkin Park", " collect@soul.alt", "Collective     Soul", " collect@soul.alt ")]
    [InlineData("Collective Soul", "linkin@park.alt", "Collective     Soul", " collect@soul.alt ")]
    public void IsSameAs_WithDifferentUser_ShouldBeDifferent(string user1Fio, string user1Email, string user2Fio, string user2Email)
    {
        var no = 1;
        var regDate = DateTime.Parse("03/04/4018 04:22:16");
        var user1 = new User { Fio = user1Fio, Email = user1Email };
        var user2 = new User { Fio = user2Fio, Email = user2Email };
        var order1 = new Order { No = no, RegDate = regDate, User = user1 };
        var order2 = new Order { No = no, RegDate = regDate, User = user2 };
        Assert.False(order1.IsSameAs(order2));
    }

    // und so weiter ......
}