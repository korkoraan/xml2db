using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace xml2db.DB;

public enum OrderState
{
    Filling,
    Pending,
    Cancelled,
    Completed,
    Deprecated
}

public class Order
{
    [Key]
    public Guid OrderId { get; set; }
    public User User { get; set; }
    public List<ProductBatch> ProductBatches { get; set; } = [];
    public int No { get; set; }
    public DateTime RegDate { get; set; }
    public OrderState State { get; set; } = OrderState.Filling; 
    [NotMapped]
    public int Sum { get; set; }
    

    public bool IsValid()
    {
        return No > 0 &&
               RegDate.Year > 33 
               && User.IsValid() 
               && ProductBatches.All(batch => batch.IsValid()) 
               && Sum == ProductBatches.Sum(batch => batch.Value);
    }

    public bool IsSameAs(Order order)
    {
        var result = User.IsSameAs(order.User) 
                     && No == order.No 
                     && RegDate == order.RegDate 
                     && State == order.State;
        if (!result) 
            return result;
        return ProductBatches.Count == order.ProductBatches.Count
               && ProductBatches.All(b1 => order.ProductBatches.Find(b2 => b2.IsSameAs(b1)) is not null)
               && order.ProductBatches.All(b1 => ProductBatches.Find(b2 => b2.IsSameAs(b1)) is not null);
    }

    public override string ToString()
    {
        var sumStr = Sum.ToString();
        var sum = sumStr.Insert(sumStr.Length - 2, ".");
        var str = $"Order #{No}: " +
                  $"\n\tClient: {User?.Fio}" +
                  $"\n\tDate: {RegDate}" +
                  $"\n\tSum: {sum}" +
                  $"\n\tProducts:\n";
        foreach (var batch in ProductBatches)
        {
            str += $"\t\t'{batch.Price.Product.Name}'\t cost: {batch.Price.Value}\t quantity: {batch.Quantity} \n";
        }

        return str;
    }
}