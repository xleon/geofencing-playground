using System;
using Splat;
using SQLite;

namespace GeofencePlayground
{
    [Table("LogEntries")]
    public class LogEntry
    {
        [AutoIncrement][PrimaryKey]
        public int Id { get; set; }

        public LogLevel LogLevel { get; set; }
        public DateTime Time { get; set; }
        public string Message { get; set; }
    }
}
