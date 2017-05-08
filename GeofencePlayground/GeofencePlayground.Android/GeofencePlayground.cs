using System;
using Android.App;
using GeofencePlayground.Droid.Geofencing;
using GeofencePlayground.Droid.Helpers;
using GeofencePlayground.Geofencing;
using Splat;

namespace GeofencePlayground.Droid
{
    [Application]
    public class GeofencePlayground : Application, IEnableLogger
    {
        private Bootstrap _app;

        public GeofencePlayground(IntPtr javaReference, Android.Runtime.JniHandleOwnership transfer) :
            base(javaReference, transfer)
        {

        }

        public override void OnCreate()
        {
            base.OnCreate();

            Locator.CurrentMutable.RegisterConstant(new LogcatLogger(), typeof(ILogger));
            Locator.CurrentMutable.Register(() => new GeofencingManager(Context), typeof(IGeofencingManager));

            _app = new Bootstrap();
        }
    }
}