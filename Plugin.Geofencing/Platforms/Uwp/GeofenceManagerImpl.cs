using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;


namespace Plugin.Geofencing
{
    //https://docs.microsoft.com/en-us/windows/uwp/maps-and-location/set-up-a-geofence
    public class GeofenceManagerImpl : IGeofenceManager
    {
        public GeofenceManagerImpl()
        {
            GeofenceMonitor.Current.GeofenceStateChanged += (sender, args) =>
            {
                if (this.RegionStatusChanged == null)
                    return;

                var changes = GeofenceMonitor
                    .Current
                    .ReadReports()
                    .Where(x => x.Geofence.Geoshape is Geocircle);

                foreach (var change in changes)
                {
                    var state = this.FromNative(change.NewState);
                    var region = this.FromNative(change.Geofence);
                    this.RegionStatusChanged?.Invoke(this, new GeofenceStatusChangedEventArgs(region, state));
                }
            };
        }


        public GeofenceManagerStatus Status
        {
            get
            {
                switch (GeofenceMonitor.Current.Status)
                {
                    case GeofenceMonitorStatus.Ready:
                        return GeofenceManagerStatus.Ready;

                    case GeofenceMonitorStatus.Disabled:
                        return GeofenceManagerStatus.Disabled;

                    case GeofenceMonitorStatus.NotAvailable:
                        return GeofenceManagerStatus.NotSupported;

                    default:
                        return GeofenceManagerStatus.Unknown;
                }
            }
        }

        public async Task<PermissionStatus> RequestPermission()
        {
            var result = await CrossPermissions
                .Current
                .RequestPermissionsAsync(Permission.LocationAlways)
                .ConfigureAwait(false);

            if (!result.ContainsKey(Permission.LocationAlways))
                return PermissionStatus.Unknown;

            return result[Permission.LocationAlways];
        }


        public IReadOnlyList<GeofenceRegion> MonitoredRegions => GeofenceMonitor
            .Current
            .Geofences
            .Where(x => x.Geoshape is Geocircle)
            .Select(this.FromNative)
            .ToList();

        //<Capabilities>
        //<!-- DeviceCapability elements must follow Capability elements(if present) -->
        //<DeviceCapability Name = "location" />
        //    </ Capabilities >
        public void StartMonitoring(GeofenceRegion region)
        {
            //var accessStatus = await Geolocator.RequestAccessAsync();
            var native = this.ToNative(region);
            GeofenceMonitor.Current.Geofences.Add(native);
        }


        public void StopMonitoring(GeofenceRegion region)
        {
            // TODO: strong guess - this thing isn't thread safe
            var list = GeofenceMonitor.Current.Geofences;
            var geofence = list.FirstOrDefault(x => x.Id.Equals(region.Identifier));

            if (geofence != null)
                list.Remove(geofence);
        }


        public void StopAllMonitoring() => GeofenceMonitor.Current.Geofences.Clear();


        public Task<GeofenceStatus> RequestState(GeofenceRegion region, CancellationToken? cancelToken = null)
        {
            // TODO
            throw new NotImplementedException();
        }

        public event EventHandler<GeofenceStatusChangedEventArgs> RegionStatusChanged;


        GeofenceStatus FromNative(GeofenceState state)
        {
            switch (state)
            {
                case GeofenceState.Entered:
                    return GeofenceStatus.Entered;

                case GeofenceState.Exited:
                    return GeofenceStatus.Exited;

                default:
                    return GeofenceStatus.Unknown;
            }
        }


        GeofenceRegion FromNative(Geofence native)
        {
            var circle = (Geocircle)native.Geoshape;
            var position = new Position(circle.Center.Latitude, circle.Center.Longitude);
            var radius = Distance.FromMeters(circle.Radius);
            return new GeofenceRegion(native.Id, position, radius)
            {
                SingleUse = native.SingleUse,
                NotifyOnEntry = native.MonitoredStates.HasFlag(MonitoredGeofenceStates.Entered),
                NotifyOnExit = native.MonitoredStates.HasFlag(MonitoredGeofenceStates.Exited)
            };
        }


        Geofence ToNative(GeofenceRegion region)
        {
            var position = new BasicGeoposition
            {
                Latitude = region.Center.Latitude,
                Longitude = region.Center.Longitude
            };

            var circle = new Geocircle(position, region.Radius.TotalMeters);
            var geofence = new Geofence(
                region.Identifier, circle,
                ToStates(region),
                region.SingleUse
            );
            return geofence;
        }


        static MonitoredGeofenceStates ToStates(GeofenceRegion region)
        {
            var i = 0;
            if (region.NotifyOnEntry)
                i = 1;
            if (region.NotifyOnExit)
                i += 2;
            return (MonitoredGeofenceStates) i;
        }
    }
}
