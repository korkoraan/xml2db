using System.Xml;
using xml2db;
using xml2db.DB;

await Main(args);
return;

async Task Main(string[] args)
{
    var xmlPath = args.Length > 0 ? args[0] : "info.xml";
    var dbPath = args.Length > 1 ? args[1] : "shop.db";

    var stream = File.OpenRead(xmlPath);
    var settings = new XmlReaderSettings
    {
        Async = true
    };
    var reader = XmlReader.Create(stream, settings);
    await using (var db = new ShopContext(dbPath))
    {
#if CLEAR_DB
        db.Database.EnsureDeleted();
#endif
        db.Database.EnsureCreated();
        OrdersExporter.OnOrderRead += db.AddOrder;

        var transaction = db.Database.BeginTransaction();
        try
        {
            OrdersExporter.ReadOrders(reader);
            transaction.Commit();
        }
        catch (Exception e)
        {
            transaction.Rollback();
            throw;
        }
    }
}

static class Log
{
    // TODO: выяснить: выпиливает ли оптимизатор вызов пустого метода
    public static void Info(object o)
    {
#if DEBUG
        Console.WriteLine(o);
#endif
    }
}