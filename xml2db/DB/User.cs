using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.CompilerServices;

namespace xml2db.DB;

[Index(nameof(Email), IsUnique = true)]
public class User
{
    [Key]
    public Guid UserId { get; set; }

    private string _fio = "";

    [MaxLength(100)]
    public string Fio
    {
        get => _fio; 
        set => _fio = Util.TrimAll(value);
    }

    private string _email = "";

    [MaxLength(100)]
    public string Email
    {
        get => _email;
        set => _email = Util.ParseEmail(value) ?? "";
    }

    public bool IsValid()
    {
        return Fio != "" && Util.IsValidEmail(Email);
    }

    public bool IsSameAs(User user)
    {
        return Fio.Equals(user.Fio) && Email.Equals(user.Email);
    }
}