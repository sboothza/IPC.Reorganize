namespace IPC.Reorganize.Json;

public class JsonSerializerOptions
{
    public bool IgnoreErrors { get; set; }
    public bool DontSerializeNulls { get; set; }
    public bool IgnoreCaseDeserializing { get; set; }
    public bool ProcessFloatsAsInts { get; set; }
    public bool DeserializeMongoDbTypes { get; set; }
    public NamingOptions Naming { get; set; } = NamingOptions.Default;
    public Dictionary<string, string> RemapFields { get; } = [];

    public static readonly JsonSerializerOptions Empty = new() { Naming = NamingOptions.Default };
}

public enum NamingOptions
{
    Default = 1,
    PascalCase = 2,
    SnakeCase = 3,
    CamelCase = 4,
    PropertyName = 5
}