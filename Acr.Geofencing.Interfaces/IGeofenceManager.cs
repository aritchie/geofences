using System;
using System.Collections.Generic;


namespace Acr.Geofencing
{

    public interface IGeofenceManager
    {
        IReadOnlyList<GeofenceRegion> MonitoredRegions { get; }
        void StartMonitoring(GeofenceRegion region);
        void StopMonitoring(GeofenceRegion region);
        void StopAllMonitoring();
        Distance DesiredAccuracy { get; set; }

        event EventHandler<GeofenceStatusChangedEventArgs> RegionStatusChanged;
    }
}