using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Acr.Geofencing {

    public interface IGeofenceManager {

        Task<bool> Initialize();

        event EventHandler<GeofenceRegion> Entered;
        event EventHandler<GeofenceRegion> Exited;

        IReadOnlyList<GeofenceRegion> MonitoredRegions { get; }
        void StartMonitoring(GeofenceRegion region);
        void StopMonitoring(GeofenceRegion region);
        void StopAllMonitoring();
    }
}
/*
 //Set the Priority for the Geofence Tracking Location Accuracy
    public static GeofencePriority GeofencePriority { get; set; }

    //Set the smallest displacement should be done from current location before a location update
    public static float SmallestDisplacement { get; set; }


              bool retVal = false;
          RequestAlwaysAuthorization();


          if (!CLLocationManager.LocationServicesEnabled)
          else if (CLLocationManager.Status == CLAuthorizationStatus.Denied || CLLocationManager.Status == CLAuthorizationStatus.Restricted)
          else if (CLLocationManager.IsMonitoringAvailable(typeof(CLRegion)))
          /... good
          else
          ...
*/