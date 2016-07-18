using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
                {
                    var native = args.Region as CLCircularRegion;
                    if (native == null)
                        return;

                    var region = this.FromNative(native);
                    ob.OnNext(new GeofenceStatusEvent(region, GeofenceStatus.Entered));
                });
                var leftHandler = new EventHandler<CLRegionEventArgs>((sender, args) =>
                {
                    var native = args.Region as CLCircularRegion;
                    if (native == null)
                        return;

                    var region = this.FromNative(native);
                    ob.OnNext(new GeofenceStatusEvent(region, GeofenceStatus.Exited));
                });
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



        protected virtual GeofenceRegion FromNative(CLCircularRegion native)
        {
            return new GeofenceRegion
            {
                Identifier = native.Identifier,
                Latitude = native.Center.Latitude,
                Longitude = native.Center.Longitude,
                Radius = Distance.FromMeters(native.Radius)
            };
        }


        protected virtual CLCircularRegion ToNative(GeofenceRegion region)
        {
            var center = new CLLocationCoordinate2D(region.Latitude, region.Longitude);
            return new CLCircularRegion(center, region.Radius.TotalMeters, region.Identifier);
        }
    }
}