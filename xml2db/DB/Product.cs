using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace xml2db.DB;

public class Product
{
    [Key]
    public Guid ProductId { get; set; }
    
    private string _name = "";

    [MaxLength(100)]
    public string Name
    {
        get => _name; 
        set => _name = Util.TrimAll(value);
    }

    public bool IsValid()
    {
        return Name != "";
    }

    public bool IsSameAs(Product product)
    {
        return Name.Equals(product.Name);
    }

}