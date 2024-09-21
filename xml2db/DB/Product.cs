using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace xml2db.DB;

public class Product
{
    [Key]
    public Guid ProductId { get; set; }
    
    [Comment("amount left in store")]
    public int Quantity { get; set; }
    
    [MaxLength(100)]
    public string Name { get; set; }

    public bool IsValid()
    {
        return Name != "";
    }
}