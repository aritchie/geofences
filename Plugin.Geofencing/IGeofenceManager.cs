using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Plugin.Permissions.Abstractions;


namespace Plugin.Geofencing
{
    public interface IGeofenceManager
    {
        /// <summary>
        /// Geofencing Status
        /// </summary>
        GeofenceManagerState Status { get; }

        /// <summary>
        /// Requests permission to use location services
        /// </summary>
        /// <returns></returns>
        Task<PermissionStatus> RequestPermission();

        /// <summary>
        /// Current set of geofences being monitored
        /// </summary>
        IReadOnlyList<GeofenceRegion> MonitoredRegions { get; }

        /// <summary>
        /// Start monitoring a geofence
        /// </summary>
        /// <param name="region"></param>
        Task StartMonitoring(GeofenceRegion region);

        /// <summary>
        /// Stop monitoring a geofence
        /// </summary>
        /// <param name="region"></param>
        Task StopMonitoring(GeofenceRegion region);

        /// <summary>
        /// Stop monitoring all active geofences
        /// </summary>
        Task StopAllMonitoring();

        /// <summary>
        /// This will request the current status of a geofence region
        /// </summary>
        /// <param name="region"></param>
        /// <param name="cancelToken"></param>
        /// <returns>Status of geofence</returns>
        Task<GeofenceState> RequestState(GeofenceRegion region, CancellationToken cancelToken = default(CancellationToken));

        /// <summary>
        /// The geofence event
        /// </summary>
        event EventHandler<GeofenceStatusChangedEventArgs> RegionStatusChanged;
    }
}