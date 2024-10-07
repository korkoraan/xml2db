using System.Xml;
using xml2db.DB;

namespace xml2db
{
    static class Tags
    {
        public static class Order
        {
            public const string TAG = "order";
            public const string NO = "no";
            public const string LIST = "orders";
            public const string REG_DATE = "reg_date";
            public const string SUM = "sum";
        }
        
        public static class User
        {
            public const string TAG = "user";
            public const string FIO = "fio";
            public const string EMAIL = "email";
        }

        public static class Product
        {
            public const string TAG = "product";
            public const string QUANTITY = "quantity";
            public const string NAME = "name";
            public const string PRICE = "price";
        }
    }
    
    public static class OrdersExporter
    {
        public delegate void NewOrderHandler(Order order);

        public static event NewOrderHandler? OnOrderRead; 
        static int ReadCurrency(XmlReader reader)
        {
            return (int)Math.Truncate(reader.ReadElementContentAsDouble() * 100);            
        }

        static void Skip2ClosingTag(string tag, XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == tag)
                    return;
            }
        }

        public static List<Order>? ReadOrders(XmlReader reader)
        {
            List<Order>? result = null;
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == Tags.Order.LIST)
                            result = ReadOrderList(reader);
                        break;
                }
            }

            return result;
        }

        static List<Order> ReadOrderList(XmlReader reader)
        {
            List<Order> result = [];
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == Tags.Order.TAG)
                        {
                            var order = ReadOrder(reader);
                            if (order is not null)
                            {
                                result.Add(order);
                                OnOrderRead?.Invoke(order);
                            }
                        }

                        break;
                    case XmlNodeType.EndElement:
                        if (reader.Name == Tags.Order.LIST)
                            return result;
                        break;
                }
            }

            return result;
        }

        static Order? ReadOrder(XmlReader reader)
        {
            var order = new Order();
            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (reader.Name)
                            {
                                case Tags.Order.NO:
                                    order.No = reader.ReadElementContentAsInt();
                                    break;
                                case Tags.Order.REG_DATE:
                                    var date = DateTime.Parse(reader.ReadElementContentAsString());
                                    order.RegDate = date;
                                    break;
                                case Tags.Order.SUM:
                                    order.Sum = ReadCurrency(reader);
                                    break;
                                case Tags.Product.TAG:
                                    var (price, quantity) = ReadProduct(reader);
                                    if (price is null)
                                        throw new InvalidXmlException("Product is corrupted");
                                    var batch = new ProductBatch
                                    {
                                        Order = order,
                                        Price = price,
                                        Quantity = quantity
                                    };

                                    order.ProductBatches.Add(batch);
                                    break;
                                case Tags.User.TAG:
                                    order.User = ReadUser(reader);
                                    if (order.User is null)
                                        throw new InvalidXmlException("User is corrupted");
                                    break;
                            }
                            break;
                        case XmlNodeType.EndElement:
                            if (reader.Name == Tags.Order.TAG)
                                return order;
                            break;
                    }
                }
            }
            catch (Xml2DbException e)
            {
                Console.WriteLine($"skip order cause is corrupted: {e.Message}");
                Skip2ClosingTag(Tags.Order.TAG, reader);
            }
            return null;
        }

        static (Price, int) ReadProduct(XmlReader reader)
        {
            var price = new Price
            {
                Product = new Product()
            };
            var quantity = 0;
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (reader.Name)
                        {
                            case Tags.Product.QUANTITY:
                                quantity = reader.ReadElementContentAsInt();
                                break;
                            case Tags.Product.NAME:
                                price.Product.Name = Util.TrimAll(reader.ReadElementContentAsString());
                                break;
                            case Tags.Product.PRICE:
                                price.Value = ReadCurrency(reader);
                                break;
                        }
                        break;
                    case XmlNodeType.EndElement:
                        if (reader.Name == Tags.Product.TAG)
                            return (price, quantity);
                        break;
                }
            }
            throw new InvalidXmlException("Product is corrupted");;
        }

        static User ReadUser(XmlReader reader)
        {
        var result = new User();
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (reader.Name)
                        {
                            case Tags.User.FIO:
                                result.Fio = Util.TrimAll(reader.ReadElementContentAsString());
                                break;
                            case Tags.User.EMAIL:
                                result.Email = reader.ReadElementContentAsString().Trim();
                                break;
                        }
                        break;
                    case XmlNodeType.EndElement:
                        if (reader.Name == Tags.User.TAG)
                            return result;
                        break;
                }
            }
            throw new InvalidXmlException("User is corrupted");;
        }
    }
}