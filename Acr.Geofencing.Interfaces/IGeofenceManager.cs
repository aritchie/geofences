using System;
using System.Collections.Generic;


namespace Acr.Geofencing
{

    public interface IGeofenceManager
    {
        // TODO: I need a way to init this, especially to check if permission is enabled
        // TODO: set accuracy
        IObservable<GeofenceStatusEvent> WhenRegionStatusChanged();
        IReadOnlyList<GeofenceRegion> MonitoredRegions { get; }
        void StartMonitoring(GeofenceRegion region);
        void StopMonitoring(GeofenceRegion region);
        void StopAllMonitoring();
    }
}