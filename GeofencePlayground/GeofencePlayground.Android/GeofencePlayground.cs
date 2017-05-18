using System;
using Android.App;
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
            GeofenceInitializer.Start();
        }
    }
}