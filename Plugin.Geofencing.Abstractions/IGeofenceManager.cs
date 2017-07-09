using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace Plugin.Geofencing
{
    public interface IGeofenceManager
    {
        /// <summary>
        /// Current set of regions being monitored
        /// </summary>
        IReadOnlyList<GeofenceRegion> MonitoredRegions { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="region"></param>
        void StartMonitoring(GeofenceRegion region);
        void StopMonitoring(GeofenceRegion region);
        void StopAllMonitoring();
        Distance DesiredAccuracy { get; set; }

        /// <summary>
        /// This will request the current status of a geofence region
        /// </summary>
        /// <param name="region"></param>
        /// <param name="cancelToken"></param>
        /// <returns>Status of geofence</returns>
        Task<GeofenceStatus> RequestState(GeofenceRegion region, CancellationToken? cancelToken = null);

        event EventHandler<GeofenceStatusChangedEventArgs> RegionStatusChanged;
    }
}