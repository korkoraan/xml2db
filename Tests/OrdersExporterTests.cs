using System.Xml;
using xml2db;

namespace Tests;

public class OrdersExporterTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void CorrectInput()
    {
        const string path = "correctInput.xml";
        var stream = File.OpenRead(path);
        var settings = new XmlReaderSettings
        {
            Async = true
        };
        using var reader = XmlReader.Create(stream, settings);
        var result = OrdersExporter.ReadOrders(reader);
        Assert.That(result != null && result.All(o => o.IsValid()));
    }

    [Test]
    public void OrderIsMissingName()
    {
        var path = "orderInvalid1.xml";
        var stream = File.OpenRead(path);
        var settings = new XmlReaderSettings
        {
            Async = true
        };
        using var reader1 = XmlReader.Create(stream, settings);
        Assert.That(() => OrdersExporter.ReadOrders(reader1).Any(o => !o.IsValid()));
    }
    
    [Test]
    public void OrderIsMissingRegDate()
    {
        var path = "orderInvalid2.xml";
        var stream = File.OpenRead(path);
        var settings = new XmlReaderSettings
        {
            Async = true
        };
        using var reader1 = XmlReader.Create(stream, settings);
        Assert.That(() => OrdersExporter.ReadOrders(reader1).Any(o => !o.IsValid()));
    }
    
    [Test]
    public void OrderIsMissingSum()
    {
        var path = "orderInvalid3.xml";
        var stream = File.OpenRead(path);
        var settings = new XmlReaderSettings
        {
            Async = true
        };
        using var reader1 = XmlReader.Create(stream, settings);
        Assert.That(() => OrdersExporter.ReadOrders(reader1).Any(o => !o.IsValid()));
    }
    
    [Test]
    public void OrderIsMissingProducts()
    {
        var path = "orderInvalid4.xml";
        var stream = File.OpenRead(path);
        var settings = new XmlReaderSettings
        {
            Async = true
        };
        using var reader1 = XmlReader.Create(stream, settings);
        Assert.That(() => OrdersExporter.ReadOrders(reader1).Any(o => !o.IsValid()));
    }
    
    [Test]
    public void OrderIsMissingUser()
    {
        var path = "orderInvalid5.xml";
        var stream = File.OpenRead(path);
        var settings = new XmlReaderSettings
        {
            Async = true
        };
        using var reader1 = XmlReader.Create(stream, settings);
        Assert.That(() => OrdersExporter.ReadOrders(reader1).Any(o => !o.IsValid()));
    }
    
    [Test]
    public void ProductIsInvalid()
    {
        var path1 = "productInvalid1.xml";
        var stream1 = File.OpenRead(path1);
        var settings = new XmlReaderSettings
        {
            Async = true
        };
        using var reader1 = XmlReader.Create(stream1, settings);
        Assert.That(() => OrdersExporter.ReadOrders(reader1).Any(o => !o.IsValid()));
        
        var path2 = "productInvalid2.xml";
        var stream2 = File.OpenRead(path2);
        using var reader2 = XmlReader.Create(stream2, settings);
        Assert.That(() => OrdersExporter.ReadOrders(reader2).Any(o => !o.IsValid()));
        
        var path3 = "productInvalid3.xml";
        var stream3 = File.OpenRead(path3);
        using var reader3 = XmlReader.Create(stream3, settings);
        Assert.That(() => OrdersExporter.ReadOrders(reader3).Any(o => !o.IsValid()));
    } 
    
    [Test]
    public void UserIsInvalid()
    {
        var path1 = "userInvalid1.xml";
        var stream1 = File.OpenRead(path1);
        var settings = new XmlReaderSettings
        {
            Async = true
        };
        using var reader1 = XmlReader.Create(stream1, settings);
        Assert.That(() => OrdersExporter.ReadOrders(reader1).Any(o => !o.IsValid()));
        
        var path2 = "userInvalid2.xml";
        var stream2 = File.OpenRead(path2);
        using var reader2 = XmlReader.Create(stream2, settings);
        Assert.That(() => OrdersExporter.ReadOrders(reader2).Any(o => !o.IsValid()));
    }
}