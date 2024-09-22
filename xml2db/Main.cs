﻿using System.Xml;
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
        db.Database.EnsureCreated();
        var transaction = db.Database.BeginTransaction();
    
        OrdersExporter.NewOrder += order =>
        {
            if (!order.IsValid())
            {
#if DEBUG
                C.Log($"Order is invalid: no {order.No}");
#endif
                return;
            }
            db.Orders.Add(order);
            db.SaveChanges();
#if DEBUG
            C.Log(order);
#endif
        };
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

static class C
{
    public static void Log(object o)
    {
        Console.WriteLine(o);
    }

    public static void Log(Order order)
    {
        var sumStr = order.Sum.ToString();
        var sum = sumStr.Insert(sumStr.Length - 2, ".");
        var str = $"Order #{order.No}: " +
                  $"\n Client: {order.User.Fio}" +
                  $"\n Date: {order.RegDate}" +
                  $"\n Sum: {sum}" +
                  $"\n Products: \n";
        foreach (var batch in order.ProductBatches)
        {
            str += $"{batch.PricedProduct.Product.Name} X{batch.Quantity} \n";
        }
        Console.WriteLine(str);
    }
}