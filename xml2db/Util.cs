using System.Text.RegularExpressions;

namespace xml2db;

public class Util
{
    public static string TrimAll(string value)
    {
        return Regex.Replace(value.Trim(), @"\s+", " ");
    }
}