# Geofencing Playground

A POC project to test GeoFencing capabilities with Xamarin

__Goals achieved (in Android at the moment):__

1. A single screen application with a real time log of geofence related events. 
2. Make sure geofences work when rebooting the device
3. Make sure geofences work when disabling and enabling back device GPS (location services)
4. Make sure we get notifications (log entries) when entering or exiting a geofence circular region
5. Persist log entries to a Sqlite database for further analysis
6. Use latest `Xamarin.GooglePlayServices` nugets (42.1021.0 atm)

__TODO__

- Button to start/stop geofencing
- A google maps view to add geofences
- Local notifications
- iOS app

__Important!__

If your app installed on external storage(SD card), 
you will never receive boot completed action (See class `GeofenceBootReceiver`). 
So you have to specify android:installLocation="internalOnly" in the manifest tag. 
This is because android device will broadcast BOOT_COMPLETED action 
before setting up the external storage.

__How to fake location on a real Android device__

There are a [bunch of apps in Google Play](https://play.google.com/store/search?q=fake%20location) that can fake the gps location. 
Just install one of them. But for any them to work you´ll need to enable it in
Android settings > Development > Choose application to simulate location

__Interesting links__

https://developer.android.com/training/location/geofencing.html
