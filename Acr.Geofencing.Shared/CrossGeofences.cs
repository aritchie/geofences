using System;


namespace Acr.Geofencing
{

    public static class CrossGeofences
    {
        static IGeofenceManager current;
        public static IGeofenceManager Current
        {
            get
            {
#if PCL
                if (current == null)
                    throw new NotSupportedException("[Acr.Geofencing] No platform plugin found.  Did you install the nuget package in your app project as well?");
#else
                current = current ?? new GeofenceManagerImpl();
#endif
                return current;
            }
            set { current = value; }
        }
    }
}
