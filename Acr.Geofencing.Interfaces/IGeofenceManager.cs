using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Acr.Geofencing {

    public interface IGeofenceManager {

        // TODO: priority
        Task<bool> Initialize();

        event EventHandler<GeofenceRegion> Entered;
        event EventHandler<GeofenceRegion> Exited;

        IReadOnlyList<GeofenceRegion> MonitoredRegions { get; }
        void StartMonitoring(GeofenceRegion region);
        void StopMonitoring(GeofenceRegion region);
        void StopAllMonitoring();
    }
}