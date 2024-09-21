namespace xml2db;

public static class TagDict
{
    public static readonly Dictionary<string, string[]> Dict;

    static TagDict()
    {
        Dict = new Dictionary<string, string[]>()
        {
            [""] = ["orders"],
            ["orders"] = ["order"],
            ["order"] = ["no", "reg_date", "sum", "product", "user"],
            ["no"] = [],
            ["reg_date"] = [],
            ["sum"] = [],
            ["product"] = ["quantity", "name", "price"],
            ["quantity"] = [],
            ["name"] = [],
            ["price"] = [],
            ["user"] = ["fio", "email"],
            ["fio"] = [],
            ["email"] = [],
        };
    }
}