using Splat;
using SQLite;

namespace GeofencePlayground
{
    public interface IDatabaseService : IEnableLogger
    {
        SQLiteConnection DefaultConnection { get; }
    }
}
