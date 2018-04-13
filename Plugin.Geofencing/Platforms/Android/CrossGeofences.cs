using System;
using Android.App;

[assembly: UsesPermission(Android.Manifest.Permission.AccessFineLocation)]
[assembly: UsesFeature("android.hardware.location.gps")]
[assembly: UsesFeature("android.hardware.location.network")]


namespace Plugin.Geofencing
{
    public static partial class CrossGeofences
    {
        static CrossGeofences()
        {
            Current = new GeofenceManagerImpl();
        }
    }
}
