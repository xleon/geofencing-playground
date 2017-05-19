using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.Util;
using Splat;

namespace GeofencePlayground.Droid.Geofencing
{
    [BroadcastReceiver(Exported = false)]
    [IntentFilter(new[] { "android.location.PROVIDERS_CHANGED" })]
    public class ProviderChangeReceiver : BroadcastReceiver, IEnableLogger
    {
        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                AndroidBootstrap.Start();

                this.Log().Info(intent.Action);

                var locationManager = (LocationManager)context.GetSystemService(Context.LocationService);
                var gpsEnabled = locationManager.IsProviderEnabled(LocationManager.GpsProvider);
                var networkEnabled = locationManager.IsProviderEnabled(LocationManager.NetworkProvider);

                this.Log().Info($"\nGPS enabled: {gpsEnabled}\nNetwork enabled: {networkEnabled}");

                if (networkEnabled)
                {
                    Task.Run(async () => await GeofencingManager.Current.StartGeofencing());
                }
            }
            catch (Exception e)
            {
                Log.Error(nameof(GeofenceBootReceiver), e.Message);
            }
        }
    }
}