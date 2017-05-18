
using System;
using Android.App;
using Android.Content;
using Android.Util;
using Splat;

namespace GeofencePlayground.Droid.Geofencing
{
    [BroadcastReceiver]
    [IntentFilter(new[]{Intent.ActionBootCompleted }, Categories = new[]{"android.intent.category.HOME"})]
    public class GeofenceBootReceiver : BroadcastReceiver, IEnableLogger
    {
        /// <summary>
        /// On boot completed restores all persisted regions
        /// </summary>
        /// <param name="context"></param>
        /// <param name="intent"></param>
        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                AndroidBootstrap.Start();

                this.Log().Info("ActionBootCompleted received. Now starting Geofencing...");

                GeofenceInitializer.Start();

            }
            catch (Exception e)
            {
                Log.Error(nameof(GeofenceBootReceiver), e.Message);
            }
        }
    }
}