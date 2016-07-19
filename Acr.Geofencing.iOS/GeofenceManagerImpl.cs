using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Diagnostics;
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


        public Distance DesiredAccuracy { get; set; } = Distance.FromKilometers(1);


        public IObservable<GeofenceStatusEvent> WhenRegionStatusChanged()
        {
            return Observable.Create<GeofenceStatusEvent>(ob =>
            {
                Debug.WriteLine("Wiring up to Geofencing events");

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
                    Debug.WriteLine("Unhooking from Geofencing events");
                    this.locationManager.RegionEntered -= enterHandler;
                    this.locationManager.RegionLeft -= leftHandler;
                };
            });
        }


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
            var native = this.ToNative(region);
            this.locationManager.StartMonitoring(native);
            //this.locationManager.DesiredAccuracy
            //this.locationManager.StartMonitoring(native, this.DesiredAccuracy.TotalMeters);
        }


        public void StopMonitoring(GeofenceRegion region)
        {
            var native = this.ToNative(region);
            this.locationManager.StopMonitoring(native);
        }


        public void StopAllMonitoring()
        {
            var natives = this
                .locationManager
                .MonitoredRegions
                .Select(x => x as CLCircularRegion)
                .Where(x => x != null)
                .ToList();
            
            foreach (var native in natives)
                this.locationManager.StopMonitoring(native);
        }


        protected virtual void DoBroadcast(IObserver<GeofenceStatusEvent> ob, CLRegionEventArgs args, GeofenceStatus status)
        {
            Debug.WriteLine("Firing geofence region event");
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