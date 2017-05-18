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
            AndroidBootstrap.Start();
            GeofenceInitializer.Start();

            this.Log().Info("Intent received");

            var geofencingEvent = GeofencingEvent.FromIntent(intent);
            if (geofencingEvent.HasError)
            {
                var errorMessage = GeofenceErrorMessages.GetErrorString(this, geofencingEvent.ErrorCode);
                this.Log().Error(errorMessage);
                return;
            }

            var geofenceTransition = geofencingEvent.GeofenceTransition;
            var geofences = geofencingEvent.TriggeringGeofences;
            var location = geofencingEvent.TriggeringLocation;

            if (geofenceTransition == Geofence.GeofenceTransitionEnter)
            {
                foreach (var geofence in geofences)
                {
                    this.Log().Info($"Entered {geofence.RequestId} at {location.Latitude}/{location.Longitude}");
                }
            }
            else if (geofenceTransition == Geofence.GeofenceTransitionExit)
            {
                foreach (var geofence in geofences)
                {
                    this.Log().Info($"Exited {geofence.RequestId} at {location.Latitude}/{location.Longitude}");
                }
            }
            else
            {
                this.Log().Error($"Geofence transition invalid type: {geofenceTransition}");
            }
        }
    }
}