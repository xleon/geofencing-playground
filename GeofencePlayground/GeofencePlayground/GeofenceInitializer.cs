using System.Threading.Tasks;
using GeofencePlayground.Geofencing;
using Splat;

namespace GeofencePlayground
{
    public class GeofenceInitializer
    {
        private static bool _initialized;

        public static void Start()
        {
            if (_initialized)
                return;

            var geofenceData = new[]
            {
                new GeofenceData
                {
                    Id = "Lidl",
                    Radius = 100,
                    Latitude = 38.832616,
                    Longitude = 0.110979
                },
                new GeofenceData
                {
                    Id = "Farmacia",
                    Radius = 100,
                    Latitude = 38.836528,
                    Longitude = 0.117524
                },
                new GeofenceData
                {
                    Id = "Casa",
                    Radius = 100,
                    Latitude = 38.813514,
                    Longitude = 0.134352
                }
            };

            var manager = Locator.CurrentMutable.GetService<IGeofencingManager>();
            manager.AddGeofenceData(geofenceData);

            Task.Run(async () => await manager.StartGeofencing());

            _initialized = true;
        }
    }
}
