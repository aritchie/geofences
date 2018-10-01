using System;


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
