using SQLite;

namespace GeofencePlayground
{
    public class DatabaseService : IDatabaseService
    {
        private readonly string _filePath;
        private SQLiteConnection _defaultDatabaseConnection;

        public SQLiteConnection DefaultConnection
        {
            get
            {
                if (_defaultDatabaseConnection != null)
                    return _defaultDatabaseConnection;

                _defaultDatabaseConnection = new SQLiteConnection(_filePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex)
                {
                    Trace = true
                };

                return _defaultDatabaseConnection;
            }
        }

        public DatabaseService(string filePath)
        {
            _filePath = filePath;

            DefaultConnection.CreateTable<LogEntry>();
        }
    }
}
