using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace xml2db.DB;

public class ProductBatch
{
    [Key] public int ProductBatchId { get; set; }

    public Order Order { get; set; }

    public Price Price { get; set; }

    public int Quantity { get; set; }

    [NotMapped]
    public int Value => Quantity * Price.Value;

    public bool IsValid()
    {
        return Quantity > 0 && Price.IsValid();
    }

    public bool IsSameAs(ProductBatch batch)
    {
        return Price.IsSameAs(batch.Price) && Quantity == batch.Quantity;
    }

}