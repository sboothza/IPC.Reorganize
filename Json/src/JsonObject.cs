using System.Text;
namespace IPC.Reorganize.Json;

public abstract class JsonObject
{
}

public class JsonObjectValue : JsonObject
{
	public JsonObjectValue()
	{
	}

	public JsonObjectValue(object value) => Value = value;
	public object? Value { get; set; }

	public override string? ToString() => Value != null ? Value.ToString() : string.Empty;
}

public class JsonObjectList : JsonObject
{
	public List<JsonObject> Array { get; set; } =
	[
	];

	public override string ToString()
	{
		var sb = new StringBuilder(1024);
		sb.Append('[');
		foreach (var item in Array)
			sb.Append(item);

		sb.Append(']');
		return sb.ToString();
	}
}

public class JsonObjectComplex : JsonObject
{
	public bool IsNull { get; set; } = false;
	public Dictionary<string, JsonObject?> Complex { get; set; } = [];

	public override string ToString()
	{
		var sb = new StringBuilder(1024);
		sb.Append('{');
		foreach (var name in Complex.Keys)
		{
			sb.Append(name);
			sb.Append(':');
			sb.Append(Complex[name]);
		}
		sb.Append('}');
		return sb.ToString();
	}
}