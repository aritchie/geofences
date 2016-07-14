using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Acr.Geofencing 
{

    public interface IGeofenceManager 
    {
        IObservable<GeofenceStatusEvent> WhenRegionStatusChanged();
        IReadOnlyList<GeofenceRegion> MonitoredRegions { get; }
        void StartMonitoring(GeofenceRegion region);
        void StopMonitoring(GeofenceRegion region);
        void StopAllMonitoring();
    }
}