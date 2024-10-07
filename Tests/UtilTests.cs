using xml2db;
using Xunit;

namespace Tests;

public class UtilTests
{
    [Theory]
    [InlineData("Ivanov Ivan", "     Ivanov   Ivan  ")]
    [InlineData("Ivanov Ivan", "     Ivanov   Ivan \t ")]
    [InlineData("Ivanov Ivan", "     Ivanov \n  Ivan \n ")]
    [InlineData("Ivanov Ivan", "  \t\n   Ivanov Ivan")]
    public void TrimAll_ShouldRemoveWhiteSpacesOnEdgesAndSquashItsInside(string expected, string raw)
    {
        Assert.Equal(expected, Util.TrimAll(raw));
    }
} 