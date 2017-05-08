
using Android.App;
using Android.Content;
using Android.Gms.Location;
using Splat;

namespace GeofencePlayground.Droid.Geofencing
{
    [Service]
    public class GeofenceTransitionsIntentService : IntentService, IEnableLogger
    {
        public GeofenceTransitionsIntentService() 
            : base(nameof(GeofenceTransitionsIntentService)) { }

        protected override void OnHandleIntent(Intent intent)
        {
            var geofencingEvent = GeofencingEvent.FromIntent(intent);
            if (geofencingEvent.HasError)
            {
                var errorMessage = GeofenceErrorMessages.GetErrorString(this, geofencingEvent.ErrorCode);
                this.Log().Error(errorMessage);
                return;
            }

            var geofenceTransition = geofencingEvent.GeofenceTransition;

            if (geofenceTransition == Geofence.GeofenceTransitionEnter)
            {
                // StartService(new Intent(this, typeof(BeaconService)));
                //mGoogleApiClient.Disconnect ();
            }
            else if (geofenceTransition == Geofence.GeofenceTransitionExit)
            {
                // StopService(new Intent(this, typeof(BeaconService)));
            }
            else
            {
                this.Log().Error($"Geofence transition invalid type: {geofenceTransition}");
            }
        }
    }
}