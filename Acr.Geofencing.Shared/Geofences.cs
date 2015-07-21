using System;


namespace Acr.Geofencing {

    public static class Geofences {

        private static readonly Lazy<IGeofenceManager> instanceLoad = new Lazy<IGeofenceManager>(() => {
#if __UNIFIED__ || __ANDROID__
            return new GeofenceManagerImpl();
#else
            throw new NotSupportedException("");
#endif
        });

        private static IGeofenceManager instance;
        public static IGeofenceManager Instance {
            get { return instance ?? instanceLoad.Value; }
            set { instance = value; }
        }
    }
}
