using System.Text.RegularExpressions;

namespace xml2db;

public static class Util
{
    public static string TrimAll(string value)
    {
        return Regex.Replace(value.Trim(), @"\s+", " ");
    }

    public static string? ParseEmail(string email)
    {
        try
        {
            return new System.Net.Mail.MailAddress(email).Address;
        }
        catch {
            return null;
        }
    }
    
    public static bool IsValidEmail(string email)
    {
        return ParseEmail(email) is not null;
    }
}