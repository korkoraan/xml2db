using System.ComponentModel.DataAnnotations;

namespace xml2db.DB;

public class Price
{
    [Key]
    public Guid PriceId { get; set; }
    public Product Product { get; set; }
    public int Value { get; set; }
    public DateTime AddedAt { get; set; }

    public bool IsValid()
    {
        return Product.IsValid() && Value > 0;
    }

    public bool IsSameAs(Price price)
    {
        return Product.IsSameAs(price.Product)
               && Value == price.Value 
               && AddedAt == price.AddedAt;
    }

}