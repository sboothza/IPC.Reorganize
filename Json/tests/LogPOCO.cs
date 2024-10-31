namespace IPC.Reorganize.Json.Tests
{
    public class Level
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }

    public class MethodItem
    {
        public MethodItem()
        {
            Parameters = [];
        }

        public string Name { get; set; }
        public List<string> Parameters { get; set; }
    }

    public class StackFrameItem
    {
        public string ClassName { get; set; }
        public string FileName { get; set; }
        public string LineNumber { get; set; }
        public MethodItem Method { get; set; }
    }

    public class LocationInfo
    {
        public LocationInfo()
        {
            StackFrames = [];
        }

        public string ClassName { get; set; }
        public string FileName { get; set; }
        public string LineNumber { get; set; }
        public string MethodName { get; set; }
        public List<StackFrameItem> StackFrames { get; set; }
    }

    public class LoggingEvent
    {
        public LoggingEvent()
        {
            Properties = [];
        }

        public Level Level { get; set; }
        public DateTime TimeStamp { get; set; }
        public string LoggerName { get; set; }
        public LocationInfo LocationInformation { get; set; }
        public object MessageObject { get; set; }
        public Exception ExceptionObject { get; set; }
        public Dictionary<string, object> Properties { get; set; }
    }

    public class MessageObjectTest
    {
        public string Message { get; set; }
        public int Id { get; set; }
    }
}