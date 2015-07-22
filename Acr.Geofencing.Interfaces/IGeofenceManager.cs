using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Acr.Geofencing {

    public interface IGeofenceManager {

        // TODO: priority
        Task<bool> Initialize();

        event EventHandler<GeofenceStatusChangedArgs> RegionStatusChanged;

        IReadOnlyList<GeofenceRegion> MonitoredRegions { get; }
        void StartMonitoring(GeofenceRegion region);
        void StopMonitoring(GeofenceRegion region);
        void StopAllMonitoring();
    }
}