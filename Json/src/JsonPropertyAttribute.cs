namespace IPC.Reorganize.Json;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class JsonPropertyAttribute(string name = "", bool ignore = false) : Attribute
{
    public string Name { get; set; } = name;
    public bool Ignore { get; set; } = ignore;
}