using System.Xml;
using xml2db;
using Xunit;

namespace Tests;

public class OrdersExporterTests
{
    XmlReader CreateXmlReader(string path)
    {
        var stream = File.OpenRead(path);
        var settings = new XmlReaderSettings
        {
            Async = true
        };
        return XmlReader.Create(stream, settings);
    }
    
    [Fact]
    public void ReadOrders_CorrectInput_ReturnsValidOrders()
    {
        var result = OrdersExporter.ReadOrders(CreateXmlReader("correctInput.xml"));
        Assert.True(result != null && result.All(o => o.IsValid()));
    }

    [Fact]
    public void ReadOrders_OrderIsMissingName_ReturnsInvalidOrders()
    {
        var path = "orderInvalid1.xml";
        var result = OrdersExporter.ReadOrders(CreateXmlReader(path));
        Assert.True(result != null && result.Any(o => !o.IsValid()));
    }
    
    [Fact]
    public void ReadOrders_OrderIsMissingRegDate_ReturnsInvalidOrders()
    {
        var path = "orderInvalid2.xml";
        var result = OrdersExporter.ReadOrders(CreateXmlReader(path));
        Assert.True(result != null && result.Any(o => !o.IsValid()));
    }
    
    [Fact]
    public void ReadOrders_OrderIsMissingSum_ReturnsInvalidOrders()
    {
        var path = "orderInvalid3.xml";
        var result = OrdersExporter.ReadOrders(CreateXmlReader(path));
        Assert.True(result != null && result.Any(o => !o.IsValid()));
    }
    
    [Fact]
    public void ReadOrders_OrderIsMissingProducts_ReturnsInvalidOrders()
    {
        var path = "orderInvalid4.xml";
        var result = OrdersExporter.ReadOrders(CreateXmlReader(path));
        Assert.True(result != null && result.Any(o => !o.IsValid()));
    }
    
    [Fact]
    public void ReadOrders_OrderIsMissingUser_ReturnsInvalidOrders()
    {
        var path = "orderInvalid5.xml";
        var result = OrdersExporter.ReadOrders(CreateXmlReader(path));
        Assert.True(result?.First().User == null);
    }
    
    [Fact]
    public void ReadOrders_ProductIsInvalid_ReturnsInvalidOrders()
    {
        var path1 = "productInvalid1.xml";
        var result = OrdersExporter.ReadOrders(CreateXmlReader(path1));
        Assert.True(result != null && result.Any(o => !o.IsValid()));
        
        var path2 = "productInvalid2.xml";
        result = OrdersExporter.ReadOrders(CreateXmlReader(path2));
        Assert.True(result != null && result.Any(o => !o.IsValid()));
        
        var path3 = "productInvalid3.xml";
        result = OrdersExporter.ReadOrders(CreateXmlReader(path3));
        Assert.True(result != null && result.Any(o => !o.IsValid()));
    } 
    
    [Fact]
    public void ReadOrders_UserIsInvalid_ReturnsInvalidOrders()
    {
        var path1 = "userInvalid1.xml";
        var result = OrdersExporter.ReadOrders(CreateXmlReader(path1));
        Assert.True(result != null && result.Any(o => !o.IsValid()));
        
        var path2 = "userInvalid2.xml";
        result = OrdersExporter.ReadOrders(CreateXmlReader(path2));
        Assert.True(result != null && result.Any(o => !o.IsValid()));
    }
}