using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;


namespace Plugin.Geofencing
{
    //https://docs.microsoft.com/en-us/windows/uwp/maps-and-location/set-up-a-geofence
    public class GeofenceManagerImpl : AbstractGeofenceManager
    {
        public GeofenceManagerImpl()
        {
            GeofenceMonitor.Current.GeofenceStateChanged += (sender, args) =>
            {
                var changes = GeofenceMonitor
                    .Current
                    .ReadReports()
                    .Where(x => x.Geofence.Geoshape is Geocircle);

                foreach (var change in changes)
                {
                    var state = this.FromNative(change.NewState);
                    var region = this.FromNative(change.Geofence);
                    this.OnGeofenceStatusChanged(region, state);
                }
            };
        }


        public override GeofenceManagerState Status
        {
            get
            {
                switch (GeofenceMonitor.Current.Status)
                {
                    case GeofenceMonitorStatus.Ready:
                        return GeofenceManagerState.Ready;

                    case GeofenceMonitorStatus.Disabled:
                        return GeofenceManagerState.Disabled;

                    case GeofenceMonitorStatus.NotAvailable:
                        return GeofenceManagerState.NotSupported;

                    default:
                        return GeofenceManagerState.Unknown;
                }
            }
        }


        public override IReadOnlyList<GeofenceRegion> MonitoredRegions => GeofenceMonitor
            .Current
            .Geofences
            .Where(x => x.Geoshape is Geocircle)
            .Select(this.FromNative)
            .ToList();

        //<Capabilities>
        //<!-- DeviceCapability elements must follow Capability elements(if present) -->
        //<DeviceCapability Name = "location" />
        //    </ Capabilities >
        public override async Task StartMonitoring(GeofenceRegion region)
        {
            await this.AssertPermission();
            var native = this.ToNative(region);
            GeofenceMonitor.Current.Geofences.Add(native);
        }


        public override Task StopMonitoring(GeofenceRegion region)
        {
            // TODO: strong guess - this thing isn't thread safe
            var list = GeofenceMonitor.Current.Geofences;
            var geofence = list.FirstOrDefault(x => x.Id.Equals(region.Identifier));

            if (geofence != null)
                list.Remove(geofence);

            return Task.CompletedTask;
        }


        public override Task StopAllMonitoring()
        {
            GeofenceMonitor.Current.Geofences.Clear();
            return Task.CompletedTask;
        }


        public override async Task<GeofenceState> RequestState(GeofenceRegion region, CancellationToken cancelToken)
        {
            await this.AssertPermission();

            var native = GeofenceMonitor.Current;
            var coords = native.LastKnownGeoposition?.Coordinate?.Point?.Position;
            if (coords == null)
                return GeofenceState.Unknown;

            var position = new Position(coords.Value.Latitude, coords.Value.Longitude);
            var result = region.IsPositionInside(position)
                ? GeofenceState.Entered
                : GeofenceState.Exited;

            return result;
        }


        GeofenceState FromNative(Windows.Devices.Geolocation.Geofencing.GeofenceState state)
        {
            switch (state)
            {
                case Windows.Devices.Geolocation.Geofencing.GeofenceState.Entered:
                    return GeofenceState.Entered;

                case Windows.Devices.Geolocation.Geofencing.GeofenceState.Exited:
                    return GeofenceState.Exited;

                default:
                    return GeofenceState.Unknown;
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
