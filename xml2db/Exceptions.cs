namespace xml2db;

public class Xml2DbException(string msg) : Exception(msg);

public class UnexpectedTagException : Xml2DbException
{
    public UnexpectedTagException(string tag) : base($"Unexpected tag reached: {tag} is not allowed") {}
    public UnexpectedTagException(string tag, string prevTag) : base($"Unexpected tag reached: '{tag}', previous tag is '{prevTag}'") {}
}

public class InvalidXmlException(string msg) : Xml2DbException(msg);

public class ProgramException(string msg) : Xml2DbException(msg);
