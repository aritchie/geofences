using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using CoreLocation;


namespace Acr.Geofencing
{

    public class GeofenceManagerImpl : IGeofenceManager
    {
        readonly CLLocationManager locationManager;


        public GeofenceManagerImpl()
        {
            this.locationManager = new CLLocationManager();
        }


        public IObservable<GeofenceStatusEvent> WhenRegionStatusChanged()
        {
            return Observable.Create<GeofenceStatusEvent>(ob =>
            {
                var enterHandler = new EventHandler<CLRegionEventArgs>((sender, args) =>
                    this.DoBroadcast(ob, args, GeofenceStatus.Entered)
                );
                var leftHandler = new EventHandler<CLRegionEventArgs>((sender, args) =>
                    this.DoBroadcast(ob, args, GeofenceStatus.Exited)
                );
                this.locationManager.RegionEntered += enterHandler;
                this.locationManager.RegionLeft += leftHandler;

                return () =>
                {
                    this.locationManager.RegionEntered += enterHandler;
                    this.locationManager.RegionLeft += leftHandler;
                };
            });
        }

        public IReadOnlyList<GeofenceRegion> MonitoredRegions => new ReadOnlyCollection<GeofenceRegion>(new List<GeofenceRegion>());
        //this.locationManager
        //    .MonitoredRegions
        //    .Where(x => x.)
        public void StartMonitoring(GeofenceRegion region)
        {
            var native = this.ToNative(region);
            this.locationManager.StartMonitoring(native); // TODO: accuracy?
        }


        public void StopMonitoring(GeofenceRegion region)
        {
            var native = this.ToNative(region);
            this.locationManager.StartMonitoring(native);
        }


        public void StopAllMonitoring()
        {
        }


        protected virtual void DoBroadcast(IObserver<GeofenceStatusEvent> ob, CLRegionEventArgs args, GeofenceStatus status)
        {
            var native = args.Region as CLCircularRegion;
            if (native == null)
                return;

            var region = this.FromNative(native);
            ob.OnNext(new GeofenceStatusEvent(region, status));
        }


        protected virtual GeofenceRegion FromNative(CLCircularRegion native)
        {
            return new GeofenceRegion
            {
                Identifier = native.Identifier,
                Center = this.FromNative(native.Center),
                Radius = Distance.FromMeters(native.Radius)
            };
        }


        protected virtual Position FromNative(CLLocationCoordinate2D native)
        {
            return new Position(native.Latitude, native.Longitude);
        }


        protected virtual CLLocationCoordinate2D ToNative(Position position)
        {
            return new CLLocationCoordinate2D(position.Latitude, position.Longitude);
        }


        protected virtual CLCircularRegion ToNative(GeofenceRegion region)
        {
            return new CLCircularRegion(
                this.ToNative(region.Center),
                region.Radius.TotalMeters,
                region.Identifier
            );
        }
    }
}