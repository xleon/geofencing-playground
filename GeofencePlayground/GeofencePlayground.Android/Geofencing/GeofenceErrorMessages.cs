using Android.Content;
using Android.Gms.Location;

namespace GeofencePlayground.Droid.Geofencing
{
    public class GeofenceErrorMessages
    {
        public static string GetErrorString(Context context, int errorCode)
        {
            switch (errorCode)
            {
                case GeofenceStatusCodes.GeofenceNotAvailable:
                    return "Geofence not available. Geolocation may be disabled in the device";

                case GeofenceStatusCodes.GeofenceTooManyGeofences:
                    return "Too many geofences";

                case GeofenceStatusCodes.GeofenceTooManyPendingIntents:
                    return "Too many pending intents";

                default:
                    return "Unknown geofence error";
            }
        }
    }
}