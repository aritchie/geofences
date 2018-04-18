using System;


namespace Plugin.Geofencing
{
    public static class CrossGeofences
    {
#if __IOS__ || __ANDROID__ || WINDOWS_UWP
        static CrossGeofences()
        {
            Current = new GeofenceManagerImpl();
        }
#endif

        static IGeofenceManager current;
        public static IGeofenceManager Current
        {
            get
            {
                if (current == null)
                    throw new NotSupportedException("[Plugin.Geofencing] No platform plugin set.  Did you install the nuget package in your app project as well?");

                return current;
            }
            set => current = value;
        }
    }
}
