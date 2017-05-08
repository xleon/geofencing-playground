namespace GeofencePlayground.Geofencing
{
    public class GeofenceData
    {
        public string Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        /// <summary>
        /// Geofence radius expressed in meters
        /// </summary>
        public float Radius { get; set; }

        /// <summary>
        /// Expiration time expressed in miliseconds
        /// </summary>
        public long Expiration { get; set; } = -1; 
    }
}
