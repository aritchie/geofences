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
        GeofenceManagerStatus Status { get; }

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
        void StartMonitoring(GeofenceRegion region);

        /// <summary>
        /// Stop monitoring a geofence
        /// </summary>
        /// <param name="region"></param>
        void StopMonitoring(GeofenceRegion region);

        /// <summary>
        /// Stop monitoring all active geofences
        /// </summary>
        void StopAllMonitoring();

        /// <summary>
        /// This will request the current status of a geofence region
        /// </summary>
        /// <param name="region"></param>
        /// <param name="cancelToken"></param>
        /// <returns>Status of geofence</returns>
        Task<GeofenceStatus> RequestState(GeofenceRegion region, CancellationToken? cancelToken = null);

        /// <summary>
        /// The geofence event
        /// </summary>
        event EventHandler<GeofenceStatusChangedEventArgs> RegionStatusChanged;
    }
}