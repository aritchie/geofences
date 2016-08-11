# ACR Geofence Plugin for Xamarin & Windows

A cross platform library for Xamarin & Windows that allows for easy geofence detection

## PLATFORMS

|Platform|Version|
| ------------------- |:------------------: |
|Xamarin.iOS|iOS 6+|
|Xamarin.Android|API 10+|
|Windows 10 UWP|10+|


## HOW TO USE

### To start monitoring

    Geofences.Instance.StartMonitoring(new GeofenceRegion 
    {
        Identifier = "My House",
        Radius = Distance.FromKilometers(1),
        Center = new Position(LATITUDE, LONGITUDE)
    });

### Wire up to notifications

    Geofences.Instance.RegionStatusChanged += (sender, args) => 
    {
        args.State // entered or exited
        args.Region // Identifier & details
    };

### Stop monitoring a region
    
    Geofences.Instance.StopMonitoring(GeofenceRegion);

    or

    Geofences.Instance.StopAllMonitoring();

## SETUP

### Android

    Add the following permissions to your app manifest OR request permissions for v6.0
    <uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
    android.permission.ACCESS_FINE_LOCATION
    android.permission.ACCESS_COARSE_LOCATION
    com.google.android.providers.gsf.permission.READ_GSERVICES
    android.permission.RECEIVE_BOOT_COMPLETED

### iOS

    Add the following to your Info.plist
	<key>NSLocationAlwaysUsageDescription</key>
	<string>Can we use your location</string>


### FAQ

* Q) Why create another geofence plugin

  A) I felt like the integration or bloat in other geofence libraries (stay, notifications, etc). I also didn't like that Google Play Services were required in Android which required your device to be online when creating the geofences.  This did not work with my requirements

* Q) Why use a cross platform GPS library for Android only
  
  A) James has done a ton of work around the Android geolocation mess.  I didn't want to duplicate this.  I just wanted to attach to an event and set the desired accuracy


