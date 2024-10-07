using Microsoft.EntityFrameworkCore;
using xml2db.DB;
using Xunit;
using Xunit.Sdk;

namespace Tests.DB;

public class ShopContextTests
{
    protected const string DB_PATH = "testShop.db";
    protected ShopContext ctx;

    protected Order TestOrder()
    {
        var regDate = DateTime.Parse("12/12/2012 12:12:12");
        var order = new Order {
            No = 1,
            RegDate = regDate,
            Sum = 100,
            User = new User { Email = "xyz@mail.ru", Fio = "Ivanov Ivan Ivanovich" },
        };
        order.ProductBatches = [
            new() {
                Order = order,
                Price = new Price { Product = new Product{ Name = "testProduct" }, Value = 100, AddedAt = regDate },
                Quantity = 1
            }
        ];
        return order;
    }

    protected Order TestOrder(int no)
    {
        var order = TestOrder();
        order.No = no;
        return order;
    }

    protected ShopContextTests()
    {
        ctx = new ShopContext(DB_PATH);
        ctx.Database.EnsureDeleted();
        ctx.Database.EnsureCreated();
    }

    public class AddOrderTests : ShopContextTests
    {
        private delegate void FunctionWithoutResult();
        private void DoInTansactionAndRollback(FunctionWithoutResult function)
        {
            var transaction = ctx.Database.BeginTransaction(); 
            try
            {
                function();
            }
            finally
            {
                transaction.Rollback(); 
            }
        }

        private List<Order> ReadOrdersFromDbWithAllRelated(IQueryable<Order> query)
        {
            return query
                .Include(o => o.User)
                .Include(o => o.ProductBatches)
                .ThenInclude(b => b.Price)
                .ThenInclude(price => price.Product)
                .ToList();
        }
        
        [Fact]
        public void CorrectOrder_ShouldBeAdded()
        {
            DoInTansactionAndRollback(() => 
            {
                var order = TestOrder();
                if (ctx.Orders.Any(o => o.No == order.No))
                    throw new Exception("db is not empty. test stopped");
                ctx.AddOrder(order);
                ctx.SaveChanges();
                ctx.ChangeTracker.Clear();

                var expected = TestOrder();
                expected.State = OrderState.Completed;
                var existing = ReadOrdersFromDbWithAllRelated(ctx.Orders.Where(o => o.No == order.No));
                Assert.Single(existing);
                Assert.True(expected.IsSameAs(existing[0]));
            });        
        }

        [Fact]
        public void NewOrderWithExistingNo_ShouldSetOldOrderStateToDeprecated()
        {
            DoInTansactionAndRollback(() =>
            {
                var oldOrder = TestOrder();
                if (ctx.Orders.Any(o => o.No == oldOrder.No))
                    throw new Exception("db is not empty. test stopped");
                ctx.AddOrder(oldOrder);
                ctx.SaveChanges();
                var newOrder = TestOrder();
                ctx.AddOrder(newOrder);
                ctx.SaveChanges();
                ctx.ChangeTracker.Clear();

                oldOrder = TestOrder();
                oldOrder.State = OrderState.Deprecated;
                newOrder = TestOrder();
                newOrder.State = OrderState.Completed;
                var expected = new List<Order> { oldOrder, newOrder }; 
                var existing = ReadOrdersFromDbWithAllRelated(ctx.Orders.Where(o => o.No == oldOrder.No));
                Assert.Equal(2, expected.Count());
                Assert.True(expected.All(o1 => existing.Find(o2 => o2.IsSameAs(o1)) is not null) 
                            && existing.All(o1 => expected.Find(o2 => o2.IsSameAs(o1)) is not null));
            });
        }
        
        [Fact]
        public void AddUser_WithSameEmail_ShouldThrow()
        {
            DoInTansactionAndRollback(() =>
            {
                var email = "sobaka@mail.ru";
                var user1 = new User { Email = email };
                var user2 = new User { Email = email };
                ctx.Users.Add(user1);
                ctx.Users.Add(user2);
                Assert.Throws<DbUpdateException>(() => ctx.SaveChanges());
            });
        }

        [Fact]
        public void PriceAtTheSameTime_ShouldSetToExistingPriceAtThatTime()
        {
            DoInTansactionAndRollback(() =>
            {
                ctx.AddOrder(TestOrder());
                ctx.SaveChanges();
                var testOrder = TestOrder();
                testOrder.RegDate = ctx.Prices.Include(price => price.Product).First().AddedAt;
                ctx.AddOrder(testOrder);
                ctx.SaveChanges();
                ctx.ChangeTracker.Clear();
                Assert.Equal(1, ctx.Prices.Count());
            });
        }
        
        [Fact]
        public void PriceWithLaterThanExisting_ShouldSetPriceToEarliestWithThatValue()
        {
            DoInTansactionAndRollback(() =>
            {
                var priceCountBefore = ctx.Prices.Count();
                ctx.AddOrder(TestOrder());
                ctx.SaveChanges();
                var order = TestOrder();
                order.RegDate = ctx.Prices.First().AddedAt.AddDays(1);
                order.No = TestOrder().No + 1;
                ctx.AddOrder(order);
                ctx.SaveChanges();
                ctx.ChangeTracker.Clear();
                Assert.Equal(1, ctx.Prices.Count() - priceCountBefore);
            });
        }
        
        [Fact]
        public void PriceEarlierThanExisting_ShouldUpdateExistingPriceToEarlierValue()
        {
            DoInTansactionAndRollback(() =>
            {
                ctx.AddOrder(TestOrder());
                ctx.SaveChanges();
                var price2Update = ctx.Prices.First();
                var price2UpdateId = price2Update.PriceId;
                var order = TestOrder();
                order.RegDate = price2Update.AddedAt.AddDays(-1);
                var expectedTime = order.RegDate;
                var newOrderNo = TestOrder().No + 1;
                order.No = newOrderNo;
                ctx.AddOrder(order);
                ctx.SaveChanges();
                ctx.ChangeTracker.Clear();
                var existing = ctx.Prices.Where((price => price.PriceId == price2UpdateId)).First();
                Assert.True(existing is not null);
                Assert.True(existing.AddedAt == expectedTime);
            });
        }

        [Fact]
        public void WithDifferentPrices_NewPricesAdded()
        {
            DoInTansactionAndRollback(() =>
            {
                var priceCountBefore = ctx.Prices.Count();
                ctx.AddOrder(TestOrder());
                ctx.SaveChanges();
                var testOrder = TestOrder();
                testOrder.ProductBatches.First().Price.Value += 1;
                testOrder.Sum += 1;
                ctx.AddOrder(testOrder);
                ctx.SaveChanges();

                testOrder = TestOrder();
                testOrder.ProductBatches.First().Price.Value += 2;
                testOrder.Sum += 2;
                testOrder.RegDate = ctx.Prices.First().AddedAt.AddDays(-1);
                ctx.AddOrder(testOrder);
                ctx.SaveChanges();

                testOrder = TestOrder();
                testOrder.ProductBatches.First().Price.Value += 3;
                testOrder.Sum += 3;
                testOrder.RegDate = ctx.Prices.First().AddedAt.AddDays(1);
                ctx.AddOrder(testOrder);
                ctx.SaveChanges();
                ctx.ChangeTracker.Clear();
                
                Assert.Equal(4, ctx.Prices.Count() - priceCountBefore);
            });
        }
    }
}   
