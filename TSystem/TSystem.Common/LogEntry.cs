using System;
using System.Collections.Generic;
using System.Text;

namespace TSystem.Common
{
    public enum LogType
    {
        Info,
        Error
    }
    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
        public LogType Type { get; set; } = LogType.Info;
    }
}
