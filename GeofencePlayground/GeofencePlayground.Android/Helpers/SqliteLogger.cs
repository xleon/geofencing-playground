using System;
using Splat;
using SQLite;

namespace GeofencePlayground.Droid.Helpers
{
    public class SqliteLogger : ILogger
    {
        public event Action<LogEntry> OnLogEntry;

        private readonly Logcat _logcat;
        private readonly SQLiteConnection _db;

        public SqliteLogger(IDatabaseService databaseService = null)
        {
            _logcat = new Logcat();

            _db = databaseService?.DefaultConnection 
                ?? Locator.Current.GetService<IDatabaseService>().DefaultConnection;

            Write($"{nameof(SqliteLogger)} created", LogLevel.Info);
        }

        public LogLevel Level { get; set; }

        public void Write(string message, LogLevel logLevel)
        {
            _logcat.Write(message, logLevel);

            var entry = new LogEntry
            {
                Time = DateTime.Now,
                LogLevel = logLevel,
                Message = message
            };

            _db.Insert(entry);

            OnLogEntry?.Invoke(entry);
        }
    }
}