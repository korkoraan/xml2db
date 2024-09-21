using System.ComponentModel.DataAnnotations;

namespace xml2db.DB;

public class Order
{
    [Key]
    public Guid OrderId { get; set; }
    public User? User { get; set; }
    public List<ProductBatch> ProductBatches { get; set; } = [];
    public int No { get; set; }
    public DateTime RegDate { get; set; }
    public int Sum { get; set; }

    public bool IsValid()
    {
        return No > 0 &&
               RegDate.Year > 33 &&
               User is not null &&
               User.IsValid() &&
               ProductBatches.All(batch => batch.Quantity > 0) &&
               Sum == ProductBatches.Sum(batch => batch.Quantity * batch.PricedProduct.Price);
    }
}