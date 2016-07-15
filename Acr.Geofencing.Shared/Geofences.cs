using System;


namespace Acr.Geofencing
{

    public static class Geofences
    {
        static readonly Lazy<IGeofenceManager> instanceLoad = new Lazy<IGeofenceManager>(() =>
        {
#if PCL
            throw new NotSupportedException("[Acr.Geofencing] No platform plugin found.  Did you install the nuget package in your app project as well?");
#else
            return new GeofenceManagerImpl();
#endif
        });


        static IGeofenceManager instance;
        public static IGeofenceManager Instance
        {
            get { return instance ?? instanceLoad.Value; }
            set { instance = value; }
        }
    }
}
