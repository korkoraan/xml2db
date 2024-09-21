using System.ComponentModel.DataAnnotations;

namespace xml2db.DB;

public class PricedProduct
{
    [Key]
    public Guid ProductDataId { get; set; }
    public Product Product { get; set; }
    public int Price { get; set; }

    public bool IsValid()
    {
        return Product.IsValid() && Price > 0;
    }
}