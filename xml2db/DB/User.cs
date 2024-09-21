using System.ComponentModel.DataAnnotations;

namespace xml2db.DB;

public class User
{
    [Key]
    public Guid UserId { get; set; }
    
    [MaxLength(100)]
    public string? Fio { get; set; }
    
    [MaxLength(100)]
    public string? Email { get; set; }
    
    private bool IsValidEmail(string email)
    {
        var trimmedEmail = email.Trim();

        if (trimmedEmail.EndsWith('.')) {
            return false;
        }
        try {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == trimmedEmail;
        }
        catch {
            return false;
        }
    }

    public bool IsValid()
    {
        return Fio != "" && Email is not null && IsValidEmail(Email);
    }
}