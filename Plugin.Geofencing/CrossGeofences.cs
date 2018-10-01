using System;


namespace Plugin.Geofencing
{
    public static partial class CrossGeofences
    {
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
