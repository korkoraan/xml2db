using xml2db.DB;
using Xunit;

namespace Tests.DB;

public class UserTests
{
    [Theory]
    [InlineData("Ivanov Ivan  ", "x@mail.com")]
    [InlineData(" Ivanov \t Ivan", "x@mail.com")]
    [InlineData("         Ivanov          Ivan    ", "x@mail.com")]
    [InlineData("\nIvanov Ivan\n", "x@mail.com")]
    public void IsValid_WithNonEmptyAfterTrimFio_ShouldBeOk(string rawName, string email)
    {
        var user = new User { Fio = rawName, Email = email };
        Assert.True(user.IsValid());
    }

    [Theory]
    [InlineData("", "x@mail.com")]
    [InlineData("  ", "x@mail.com")]
    [InlineData(" \t \n ", "x@mail.com")]
    public void IsValid_WithNonEmptyAfterTrimFio_ShouldBeFalse(string rawName, string email)
    {
        var user = new User { Fio = rawName, Email = email };
        Assert.False(user.IsValid());
    }

    [Theory]
    [InlineData("Bob", "fdsa")]
    [InlineData("Bob", "fdsa@")]
    public void IsValid_WithNonEmptyEmail_ShouldBeFalse(string rawName, string email)
    {
        var user = new User { Fio = rawName, Email = email };
        Assert.False(user.IsValid());
    }

    [Theory]
    [InlineData("Bob", "someone@somewhere.com")]
    [InlineData("Bob", "someone@somewhere.co.uk")]
    [InlineData("Bob", "someone+tag@somewhere.net")]
    [InlineData("Bob", "futureTLD@somewhere.fooo")]
    public void IsValid_WithNonEmptyEmail_ShouldBeTrue(string rawName, string email)
    {
        var user = new User { Fio = rawName, Email = email };
        Assert.True(user.IsValid());
    }

    [Theory]
    [InlineData("Ivan", "Ivan")]
    [InlineData("   Ivan  ", "Ivan")]
    [InlineData("Ivan \n", "Ivan")]
    [InlineData("Ivan \t", "Ivan")]
    public void IsSameAs_WithSameFio_ShouldBeSame(string rawName1, string rawName2)
    {
        var user1 = new User { Fio = rawName1 };
        var user2 = new User { Fio = rawName2 };
        Assert.True(user1.IsSameAs(user2));
    }
    
    [Theory]
    [InlineData("Ivann", "Ivan")]
    [InlineData("IvanIvanov", "Ivan Ivanov")]
    public void IsSameAs_WithDifferentFio_ShouldBeDifferent(string rawName1, string rawName2)
    {
        var user1 = new User { Fio = rawName1 };
        var user2 = new User { Fio = rawName2 };
        Assert.False(user1.IsSameAs(user2));
    }
    
    [Theory]
    [InlineData("xyz@gmail", "xyz@gmail")]
    [InlineData("  xyz@gmail  ", "xyz@gmail")]
    public void IsSameAs_WithSameEmail_ShouldBeSame(string rawEmail1, string rawEmail2)
    {
        var user1 = new User { Email = rawEmail1 };
        var user2 = new User { Email = rawEmail2 };
        Assert.True(user1.IsSameAs(user2));
    }
    
    [Theory]
    [InlineData("xyz@gmail", "zyx@gmail")]
    [InlineData("xyz@gmail", "")]
    public void IsSameAs_WithDifferentEmail_ShouldBeDifferent(string rawEmail1, string rawEmail2)
    {
        var user1 = new User { Email = rawEmail1 };
        var user2 = new User { Email = rawEmail2 };
        Assert.False(user1.IsSameAs(user2));
    }
}