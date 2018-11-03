using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CoreLocation;
using Foundation;
using UIKit;


namespace Plugin.Geofencing
{
    public class GeofenceManagerImpl : AbstractGeofenceManager
    {
        readonly CLLocationManager locationManager;

        public GeofenceManagerImpl()
        {
            this.locationManager = new CLLocationManager();
            this.locationManager.RegionEntered += (sender, args) => this.DoBroadcast(args, GeofenceState.Entered);
            this.locationManager.RegionLeft += (sender, args) => this.DoBroadcast(args, GeofenceState.Exited);
        }


        public override GeofenceManagerState Status
        {
            get
            {
                if (!CLLocationManager.LocationServicesEnabled)
                    return GeofenceManagerState.Disabled;

                if (!CLLocationManager.IsMonitoringAvailable(typeof(CLCircularRegion)))
                    return GeofenceManagerState.Disabled;

                if (CLLocationManager.Status != CLAuthorizationStatus.AuthorizedAlways)
                    return GeofenceManagerState.PermissionDenied;

                return GeofenceManagerState.Ready;
            }
        }


        public override async Task<GeofenceState> RequestState(GeofenceRegion region, CancellationToken cancelToken)
        {
            await this.AssertPermission();
            var tcs = new TaskCompletionSource<GeofenceState>();

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


        public override IReadOnlyList<GeofenceRegion> MonitoredRegions
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


        public override async Task StartMonitoring(GeofenceRegion region)
        {
            await this.AssertPermission();

            var tcs = new TaskCompletionSource<object>();
            //if !CLLocationManager.isMonitoringAvailableForClass(CLCircularRegion) {
            UIApplication.SharedApplication.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    var native = this.ToNative(region);
                    this.locationManager.StartMonitoring(native);
                    this.SetIfSingleUse(region);
                    tcs.SetResult(null);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            await tcs.Task.ConfigureAwait(false);
        }


        public override Task StopMonitoring(GeofenceRegion region)
        {
            var native = this.ToNative(region);
            this.KillIfSingleUse(region.Identifier);
            this.locationManager.StopMonitoring(native);
            return Task.CompletedTask;
        }


        public override Task StopAllMonitoring()
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
            return Task.CompletedTask;
        }


        protected virtual void DoBroadcast(CLRegionEventArgs args, GeofenceState status)
        {
            Debug.WriteLine("Firing geofence region event");
            var native = args.Region as CLCircularRegion;
            if (native == null)
                return;

            var region = this.FromNative(native);
            this.OnGeofenceStatusChanged(region, status);
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


        protected virtual GeofenceState FromNative(CLRegionState state)
        {
            switch (state)
            {
                case CLRegionState.Inside:
                    return GeofenceState.Entered;

                case CLRegionState.Outside:
                    return GeofenceState.Exited;

                case CLRegionState.Unknown:
                default:
                    return GeofenceState.Unknown;
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