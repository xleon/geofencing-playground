using System;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using GeofencePlayground.Droid.Geofencing;
using Splat;

namespace GeofencePlayground.Droid
{
    [Application]
    public class GeofencePlayground : Application, IEnableLogger
    {
        public GeofencePlayground(IntPtr javaReference, Android.Runtime.JniHandleOwnership transfer) :
            base(javaReference, transfer) { }

        public override void OnCreate()
        {
            base.OnCreate();

            AndroidBootstrap.Start();

            this.Log().Info("Start from Main Application");

            GeofencingManager.Current.AddGeofenceData(GeofenceData.Data.ToArray());

            Task.Run(async () => await GeofencingManager.Current.StartGeofencing());
        }
    }
}