using System.Xml;
using xml2db;

namespace Tests;

public class OrdersExporterTests
{
    [SetUp]
    public void Setup()
    {
    }

    XmlReader CreateXmlReader(string path)
    {
        var stream = File.OpenRead(path);
        var settings = new XmlReaderSettings
        {
            Async = true
        };
        return XmlReader.Create(stream, settings);
    }
    
    [Test]
    public void CorrectInput()
    {
        var result = OrdersExporter.ReadOrders(CreateXmlReader("correctInput.xml"));
        Assert.That(result != null && result.All(o => o.IsValid()));
    }

    [Test]
    public void OrderIsMissingName()
    {
        var path = "orderInvalid1.xml";
        var result = OrdersExporter.ReadOrders(CreateXmlReader(path));
        Assert.That(() => result != null && result.Any(o => !o.IsValid()));
    }
    
    [Test]
    public void OrderIsMissingRegDate()
    {
        var path = "orderInvalid2.xml";
        var result = OrdersExporter.ReadOrders(CreateXmlReader(path));
        Assert.That(() => result != null && result.Any(o => !o.IsValid()));
    }
    
    [Test]
    public void OrderIsMissingSum()
    {
        var path = "orderInvalid3.xml";
        var result = OrdersExporter.ReadOrders(CreateXmlReader(path));
        Assert.That(() => result != null && result.Any(o => !o.IsValid()));
    }
    
    [Test]
    public void OrderIsMissingProducts()
    {
        var path = "orderInvalid4.xml";
        var result = OrdersExporter.ReadOrders(CreateXmlReader(path));
        Assert.That(() => result != null && result.Any(o => !o.IsValid()));
    }
    
    [Test]
    public void OrderIsMissingUser()
    {
        var path = "orderInvalid5.xml";
        var result = OrdersExporter.ReadOrders(CreateXmlReader(path));
        Assert.That(() => result != null && result.Any(o => !o.IsValid()));
    }
    
    [Test]
    public void ProductIsInvalid()
    {
        var path1 = "productInvalid1.xml";
        var result = OrdersExporter.ReadOrders(CreateXmlReader(path1));
        Assert.That(() => result != null && result.Any(o => !o.IsValid()));
        
        var path2 = "productInvalid2.xml";
        result = OrdersExporter.ReadOrders(CreateXmlReader(path2));
        Assert.That(() => result != null && result.Any(o => !o.IsValid()));
        
        var path3 = "productInvalid3.xml";
        result = OrdersExporter.ReadOrders(CreateXmlReader(path3));
        Assert.That(() => result != null && result.Any(o => !o.IsValid()));
    } 
    
    [Test]
    public void UserIsInvalid()
    {
        var path1 = "userInvalid1.xml";
        var result = OrdersExporter.ReadOrders(CreateXmlReader(path1));
        Assert.That(() => result != null && result.Any(o => !o.IsValid()));
        
        var path2 = "userInvalid2.xml";
        result = OrdersExporter.ReadOrders(CreateXmlReader(path2));
        Assert.That(() => result != null && result.Any(o => !o.IsValid()));
    }
}