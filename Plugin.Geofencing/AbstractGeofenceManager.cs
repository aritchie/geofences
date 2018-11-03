using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;


namespace Plugin.Geofencing
{
    public abstract class AbstractGeofenceManager : IGeofenceManager
    {
        public event EventHandler<GeofenceStatusChangedEventArgs> RegionStatusChanged;
        public abstract GeofenceManagerState Status { get; }
        public abstract IReadOnlyList<GeofenceRegion> MonitoredRegions { get; }
        public abstract Task StartMonitoring(GeofenceRegion region);
        public abstract Task StopMonitoring(GeofenceRegion region);
        public abstract Task StopAllMonitoring();

        public abstract Task<GeofenceState> RequestState(GeofenceRegion region,
            CancellationToken cancelToken = default(CancellationToken));


        public virtual async Task<PermissionStatus> RequestPermission()
        {
            var result = await CrossPermissions
                .Current
                .RequestPermissionsAsync(Permission.LocationAlways)
                .ConfigureAwait(false);

            if (!result.ContainsKey(Permission.LocationAlways))
                return PermissionStatus.Unknown;

            return result[Permission.LocationAlways];
        }


        protected virtual async Task AssertPermission()
        {
            var permission = await this.RequestPermission();
            if (permission != PermissionStatus.Granted)
                throw new ArgumentException("Invalid permission status - " + permission);
        }


        protected virtual void OnGeofenceStatusChanged(GeofenceRegion region, GeofenceState status)
            => this.RegionStatusChanged?.Invoke(this, new GeofenceStatusChangedEventArgs(region, status));
    }
}
