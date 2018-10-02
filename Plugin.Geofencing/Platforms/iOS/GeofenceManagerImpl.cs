using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CoreLocation;
using Foundation;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using UIKit;


namespace Plugin.Geofencing
{

    public class GeofenceManagerImpl : IGeofenceManager
    {
        readonly CLLocationManager locationManager;

        public GeofenceManagerImpl()
        {
            this.locationManager = new CLLocationManager();
            this.locationManager.RegionEntered += (sender, args) => this.DoBroadcast(args, GeofenceStatus.Entered);
            this.locationManager.RegionLeft += (sender, args) => this.DoBroadcast(args, GeofenceStatus.Exited);
        }


        public GeofenceManagerStatus Status
        {
            get
            {
                if (!CLLocationManager.LocationServicesEnabled)
                    return GeofenceManagerStatus.Disabled;

                if (!CLLocationManager.IsMonitoringAvailable(typeof(CLCircularRegion)))
                    return GeofenceManagerStatus.Disabled;

                if (CLLocationManager.Status != CLAuthorizationStatus.AuthorizedAlways)
                    return GeofenceManagerStatus.PermissionDenied;

                return GeofenceManagerStatus.Ready;
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


        public async Task<GeofenceStatus> RequestState(GeofenceRegion region, CancellationToken cancelToken)
        {
            var tcs = new TaskCompletionSource<GeofenceStatus>();

            var handler = new EventHandler<CLRegionStateDeterminedEventArgs>((sender, args) =>
            {
                var clregion = args.Region as CLCircularRegion;
                if (clregion?.Identifier.Equals(region.Identifier) ?? false)
                {
                    var state = this.FromNative(args.State);
                    tcs.TrySetResult(state);
                }
            });

            try
            {
                using (cancelToken.Register(() => tcs.TrySetCanceled()))
                {
                    this.locationManager.DidDetermineState += handler;
                    var native = this.ToNative(region);
                    this.locationManager.RequestState(native);
                    return await tcs.Task;
                }
            }
            finally
            {
                this.locationManager.DidDetermineState -= handler;
            }
        }


        public event EventHandler<GeofenceStatusChangedEventArgs> RegionStatusChanged;


        public IReadOnlyList<GeofenceRegion> MonitoredRegions
        {
            get
            {
                var list = this
                    .locationManager
                    .MonitoredRegions
                    .Select(x => x as CLCircularRegion)
                    .Where(x => x != null)
                    .Select(this.FromNative)
                    .ToList();

                return new ReadOnlyCollection<GeofenceRegion>(list);
            }
        }


        public void StartMonitoring(GeofenceRegion region)
        {
            //if !CLLocationManager.isMonitoringAvailableForClass(CLCircularRegion) {
            UIApplication.SharedApplication.InvokeOnMainThread(() =>
            {
                try
                {
                    var native = this.ToNative(region);
                    this.locationManager.StartMonitoring(native);
                    this.SetIfSingleUse(region);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            });
        }


        public void StopMonitoring(GeofenceRegion region)
        {
            var native = this.ToNative(region);
            this.KillIfSingleUse(region.Identifier);
            this.locationManager.StopMonitoring(native);
        }


        public void StopAllMonitoring()
        {
            var natives = this
                .locationManager
                .MonitoredRegions
                .OfType<CLCircularRegion>()
                .ToList();

            foreach (var native in natives)
            {
                this.KillIfSingleUse(native.Identifier);
                this.locationManager.StopMonitoring(native);
            }
        }


        protected virtual void DoBroadcast(CLRegionEventArgs args, GeofenceStatus status)
        {
            Debug.WriteLine("Firing geofence region event");
            var native = args.Region as CLCircularRegion;
            if (native == null)
                return;

            var region = this.FromNative(native);
            this.RegionStatusChanged?.Invoke(this, new GeofenceStatusChangedEventArgs(region, status));
            var kill = this.KillIfSingleUse(region.Identifier);
            if (kill)
                this.StopMonitoring(region);
        }


        protected virtual GeofenceRegion FromNative(CLCircularRegion native)
        {
            var radius = Distance.FromMeters(native.Radius);
            var center = this.FromNative(native.Center);
            return new GeofenceRegion(native.Identifier, center, radius)
            {
                NotifyOnEntry = native.NotifyOnEntry,
                NotifyOnExit = native.NotifyOnExit
            };
        }


        protected virtual GeofenceStatus FromNative(CLRegionState state)
        {
            switch (state)
            {
                case CLRegionState.Inside:
                    return GeofenceStatus.Entered;

                case CLRegionState.Outside:
                    return GeofenceStatus.Exited;

                case CLRegionState.Unknown:
                default:
                    return GeofenceStatus.Unknown;
            }
        }


        NSString ToSuKey(string identifier) => new NSString(identifier + "_su");

        protected virtual bool KillIfSingleUse(string identifier)
        {
            var key = this.ToSuKey(identifier);
            var exists = NSUserDefaults.StandardUserDefaults.ValueForKey(key) != null;
            if (exists)
                NSUserDefaults.StandardUserDefaults.RemoveObject(key);

            return exists;
        }


        protected virtual void SetIfSingleUse(GeofenceRegion region)
        {
            if (region.SingleUse)
                NSUserDefaults.StandardUserDefaults.SetValueForKey(new NSString("set"), this.ToSuKey(region.Identifier));
        }


        protected virtual Position FromNative(CLLocationCoordinate2D native)
            => new Position(native.Latitude, native.Longitude);


        protected virtual CLLocationCoordinate2D ToNative(Position position)
            => new CLLocationCoordinate2D(position.Latitude, position.Longitude);


        protected virtual CLCircularRegion ToNative(GeofenceRegion region)
            => new CLCircularRegion
            (
                this.ToNative(region.Center),
                region.Radius.TotalMeters,
                region.Identifier
            )
            {
                NotifyOnExit = region.NotifyOnExit,
                NotifyOnEntry = region.NotifyOnEntry
            };
    }
}