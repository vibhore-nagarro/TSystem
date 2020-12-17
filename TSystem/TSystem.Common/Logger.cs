using System;
using System.Collections.Generic;
using System.Text;

namespace TSystem.Common
{
    
    public class LogEventArgs : EventArgs
    {
        public LogEntry LogEntry { get; set; }
    }
    public static class Logger
    {
        public delegate void OnLogHandler(Object sender, LogEventArgs args);
        public static event OnLogHandler OnLog;
        public static List<LogEntry> Logs = new List<LogEntry>();

        private static void OnLOgEvent(LogEntry logEntry)
        {
            OnLog?.Invoke(null, new LogEventArgs() { LogEntry = logEntry });
        }

        public static void Log(LogEntry log)
        {
            Logs.Add(log);
            OnLOgEvent(log);
        }
        public static void Log(string log)
        {
            Logs.Add(new LogEntry() { Message = log, Timestamp = DateTime.Now, Type = LogType.Info });
            OnLOgEvent(new LogEntry() { Message = log, Timestamp = DateTime.Now, Type = LogType.Info });
        }
    }
}
