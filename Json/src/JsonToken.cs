namespace IPC.Reorganize.Json;

public class JsonToken
{
    protected JsonToken()
    {
    }
}

public class StringToken(string value) : JsonToken
{
    public string Value { get; set; } = value;
    public override string ToString() => $"str:{Value}";
}

public class NumberToken(string value) : JsonToken
{
    public string Value { get; set; } = value;
    public override string ToString() => $"num:{Value}";
}

public class UnquotedConstantToken(string value) : JsonToken
{
    public string Value { get; set; } = value;
}

public class ObjectStartToken : JsonToken
{
}

public class ObjectEndToken : JsonToken
{
}

public class ListStartToken : JsonToken
{
}

public class ListEndToken : JsonToken
{
}

public class MemberSeparatorToken : JsonToken
{
}

public class CommaToken : JsonToken
{
}