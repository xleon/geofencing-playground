using System.Collections.Generic;
using GeofencePlayground.Geofencing;

namespace GeofencePlayground
{
    public class GeofenceData
    {
        public static IEnumerable<GeofenceRegion> Data => new[]
        {
            new GeofenceRegion
            {
                Id = "Lidl",
                Radius = 100,
                Latitude = 38.832616,
                Longitude = 0.110979
            },
            new GeofenceRegion
            {
                Id = "Farmacia",
                Radius = 100,
                Latitude = 38.836528,
                Longitude = 0.117524
            }
        };
    }
}
