using System;

namespace DotNetInsights.Shared.Domains
{
    public class LogEntry
    {
        public string Category {get; set; }
        public int LogLevelId {get; set; }
        public int EventId { get; set; }
        public string EventName { get; set; }
        public string FormattedString { get; set; }
        public DateTime Created { get; set; }
    }
}
