using System;
using System.IO;
using GeofencePlayground.Droid.Helpers;
using Splat;

namespace GeofencePlayground.Droid
{
    public static class AndroidBootstrap
    {
        private static bool _initialized;

        public static void Start()
        {
            if (_initialized)
                return;

            var databaseFilePath = Path
                .Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "geofence.db");

            var dbService = new DatabaseService(databaseFilePath);

            Locator.CurrentMutable.RegisterConstant(dbService, typeof(IDatabaseService));
            Locator.CurrentMutable.RegisterConstant(new SqliteLogger(), typeof(ILogger));
            //Locator.CurrentMutable.Register(() => new GeofencingManager(), typeof(IGeofencingManager));

            _initialized = true;
        }
    }
}