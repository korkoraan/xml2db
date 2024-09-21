using System.ComponentModel.DataAnnotations;

namespace xml2db.DB;

public class ProductBatch
{
    [Key]
    public int Id { get; set; }

    public Order Order { get; set; }

    public PricedProduct PricedProduct { get; set; }

    public int Quantity;
}