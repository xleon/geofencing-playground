# Geofencing Playground

A POC project to test GeoFencing capabilities with Xamarin

Goals achieved (in Android at the moment): 

1. A single screen application with a real time log of geofence related events. 
2. Make sure geofences work when rebooting the device
3. Make sure geofences work when disabling and enabling back device GPS (location services)
4. Make sure we get notifications (log entries) when entering or exiting a geofence circular region
5. Persist log entries to a Sqlite database for further analysis

TODO

- A google maps view to add geofences
- Local notifications
- iOS app

Important!

If your app installed on external storage(SD card), you will never receive boot completed action (See class `GeofenceBootReceiver`). So you have to specify android:installLocation="internalOnly" in the manifest tag. This is because android device will broadcast BOOT_COMPLETED action before setting up the external storage.