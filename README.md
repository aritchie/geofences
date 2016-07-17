
I disliked the need for google play services and the complexity around that for a simple geofence, so I have made my own.
Why use the geolocator library?
* James has done a ton of work around the Android geolocation mess
* It covers the use-cases for UWP as well

Why not use the other plugin library?
* Way over complicated
* Covers cases that had nothing to do with geofences such as notifications
* Based on interfaces pluging into events, not actual events

Why are using Reactive Extensions
* They are just better than traditional events for so many reasons.
* I use an old version as 3.0 does not support PCL any longer.  NetStandard, at this time (July 2016) is not ready for primetime consumption in my opinion.

Android
Mainfest
    <uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
android.permission.ACCESS_FINE_LOCATION
android.permission.ACCESS_COARSE_LOCATION
com.google.android.providers.gsf.permission.READ_GSERVICES
android.permission.RECEIVE_BOOT_COMPLETED


iOS

Info.plist
	<key>NSLocationAlwaysUsageDescription</key>
	<string>Can we use your location</string>
	<key>NSLocationWhenInUseUsageDescription</key>
	<string>We are using your location</string>