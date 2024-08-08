namespace IPC.Reorganize.Json;

class PropertyTuple
{
	public string Name { get; }
	public string OutputName { get; set; }
	public object? Value { get; }

	private PropertyTuple(object source, string name, string outputName = "")
	{
		Name = name;
		if (outputName == "")
			outputName = name;

		Value = source.GetFieldOrPropertyValue(Name);
		OutputName = outputName;
	}

	public static PropertyTuple? Create(JsonSerializerOptions options, object source, string name, string outputName = "")
	{
		try
		{
			return new PropertyTuple(source, name, outputName);
		}
		catch
		{
			if (options.IgnoreErrors)
				return null;
			throw;
		}
	}
}