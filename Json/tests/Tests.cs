using System.Text.Json.Serialization;

using IPC.Reorganize.Json;

using Newtonsoft.Json;

using NUnit.Framework;
using NUnit.Framework.Legacy;
namespace IPC.Reorganize.Json.Tests
{	
	public enum MyEnum
	{
		Value1,
		Value2
	}

	public class Sample
	{
		public string Name { get; set; }
		public long Id { get; set; }
		public List<SampleChild> Children { get; set; } = [];
	}

	public class SampleChild
	{
		public string Name { get; set; }
		public long Id { get; set; }
		public MyEnum EnumTest { get; set; }
		public bool BoolValue { get; set; }
		[JsonProperty(Ignore = true)]
		public string ShouldIgnore { get; set; }
	}

	[TestFixture]
	public class Tests
	{
		private LoggingEvent _event;
		private string _json;
		private const int LOOPS = 1000;

		[SetUp]
		public void Setup()
		{
			_event = new LoggingEvent
			{
				Level = new Level
				{
					Name = "ERROR",
					Value = 1000
				},
				LoggerName = "Test",
				TimeStamp = DateTime.Now,
				MessageObject = new MessageObjectTest
				{
					Message = "this is a message",
					Id = 1
				},
				ExceptionObject = new ArgumentOutOfRangeException(paramName: "item", message: "item not in range"),
				LocationInformation = new LocationInfo
				{
					ClassName = "JsonTests.NewtonsoftTests",
					FileName = "c:\\temp\\file.cs",
					LineNumber = "12",
					MethodName = "Test",
					StackFrames =
					{
						new StackFrameItem
						{
							ClassName = "JsonTests.NewtonsoftTests",
							FileName = "c:\\temp\\file.cs",
							LineNumber = "12",
							Method = new MethodItem
							{
								Name = "Test",
								Parameters =
								{
									"param1",
									"param2"
								}
							}
						},
						new StackFrameItem
						{
							ClassName = "JsonTests.NewtonsoftTests",
							FileName = "c:\\temp\\file.cs",
							LineNumber = "13",
							Method = new MethodItem
							{
								Name = "Test2",
								Parameters =
								{
									"param3",
									"param4"
								}
							}
						}
					}
				},
				Properties =
				{
					{
						"key1", "value1"
					},
					{
						"key2", "value2"
					},
					{
						"key3", "value3"
					}
				}
			};

			_json =
				"{\"Level\":{\"Name\":\"ERROR\",\"Value\":1000},\"TimeStamp\":\"2021-10-11T19:24:59.9788744+02:00\",\"LoggerName\":\"Test\",\"LocationInformation\":{\"ClassName\":\"JsonTests.NewtonsoftTests\",\"FileName\":\"c:\\temp\\file.cs\",\"LineNumber\":\"12\",\"MethodName\":\"Test\",\"StackFrames\":[{\"ClassName\":\"JsonTests.NewtonsoftTests\",\"FileName\":\"c:\\temp\\file.cs\",\"LineNumber\":\"12\",\"Method\":{\"Name\":\"Test\",\"Parameters\":[\"param1\",\"param2\"]}},{\"ClassName\":\"JsonTests.NewtonsoftTests\",\"FileName\":\"c:\\temp\\file.cs\",\"LineNumber\":\"13\",\"Method\":{\"Name\":\"Test2\",\"Parameters\":[\"param3\",\"param4\"]}}]},\"MessageObject\":{\"Message\":\"this is a message\",\"Id\":1},\"ExceptionObject\":{\"Message\":\"item not in range (Parameter 'item')\",\"ActualValue\":null,\"ParamName\":\"item\",\"TargetSite\":null,\"StackTrace\":null,\"Data\":{},\"InnerException\":null,\"HelpLink\":null,\"Source\":null,\"HResult\":-2146233086},\"Properties\":{\"key1\":\"value1\",\"key2\":\"value2\",\"key3\":\"value3\"}}";
		}

		//453
		[Test]
		public void TestCustom()
		{
			var json = JsonSerializer.Serialize(_event.LocationInformation);
			Console.WriteLine(json.Length);
		}

		//25.81
		[Test]
		public void MeasureCustomSerialize()
		{
			var start = DateTime.Now;
			for (var i = 0; i < LOOPS; i++)
			{
				var json = JsonSerializer.Serialize(_event, new JsonSerializerOptions());
				if (json.Length < 0)
					throw new InvalidOperationException("broke");
			}

			var spent = DateTime.Now - start;
			Console.WriteLine($"{spent.TotalMilliseconds:0.00}");
		}

		//35.89
		[Test]
		public void MeasureNewtonsoftSerialize()
		{
			var start = DateTime.Now;
			for (var i = 0; i < LOOPS; i++)
			{
				var json = JsonConvert.SerializeObject(_event);
				if (json.Length < 0)
					throw new InvalidOperationException("broke");
			}

			var spent = DateTime.Now - start;
			Console.WriteLine($"{spent.TotalMilliseconds:0.00}");
		}

		//30.74
		[Test]
		public void MeasureMicrosoftSerialize()
		{
			var start = DateTime.Now;
			var options = new System.Text.Json.JsonSerializerOptions
			{
				DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
			};

			for (var i = 0; i < LOOPS; i++)
			{
				var json = System.Text.Json.JsonSerializer.Serialize(_event, options);
				if (json.Length < 0)
					throw new InvalidOperationException("broke");
			}

			var spent = DateTime.Now - start;
			Console.WriteLine($"{spent.TotalMilliseconds:0.00}");
		}

		


		[Test]
		public void TestNotPretty()
		{
			var item = new Sample
			{
				Name = "Sample",
				Id = 12,
				Children =
				{
					new SampleChild
					{
						Id = 34,
						Name = "Child Name",
						EnumTest = MyEnum.Value2,
						BoolValue = true
					}
				}
			};
			var json = JsonSerializer.Serialize(item, new JsonSerializerOptions
			{
				IgnoreErrors = true
			});
			var jsonToMatch =
				"{\"Name\" : \"Sample\",\"Id\" : 12,\"Children\" : [{\"Name\" : \"Child Name\",\"Id\" : 34,\"EnumTest\" : \"Value2\",\"BoolValue\" : \"True\"}]}";

			Console.WriteLine(json);
			ClassicAssert.AreEqual(json, jsonToMatch);
		}

		[Test]
		public void TestNulls()
		{
			var item = new Sample
			{
				Name = "Sample",
				Id = 12,
				Children =
				{
					new SampleChild
					{
						Id = 34,
						Name = null,
						EnumTest = MyEnum.Value2,
						BoolValue = true
					}
				}
			};
			var json = JsonSerializer.Serialize(item, new JsonSerializerOptions
			{
				IgnoreErrors = true,
				DontSerializeNulls = true
			});
			var jsonToMatch =
				"{\"Name\" : \"Sample\",\"Id\" : 12,\"Children\" : [{\"Id\" : 34,\"EnumTest\" : \"Value2\",\"BoolValue\" : \"True\"}]}";

			Console.WriteLine(json);
			ClassicAssert.AreEqual(json, jsonToMatch);

			item = new Sample
			{
				Name = "Sample",
				Id = 12,
				Children = null
			};
			json = JsonSerializer.Serialize(item, new JsonSerializerOptions
			{
				IgnoreErrors = true,
				DontSerializeNulls = true
			});
			jsonToMatch = "{\"Name\" : \"Sample\",\"Id\" : 12}";

			Console.WriteLine(json);
			ClassicAssert.AreEqual(json, jsonToMatch);
		}

		
	}
}