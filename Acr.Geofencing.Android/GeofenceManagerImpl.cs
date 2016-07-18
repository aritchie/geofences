using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;


namespace Acr.Geofencing
{
    public class GeofenceManagerImpl : IGeofenceManager
    {
        readonly IGeolocator geolocator;
        readonly GeofenceSettings settings;
        readonly IDictionary<string, GeofenceState> states;
        readonly IObservable<GeofenceStatusEvent> observe;
        Position current;


        public GeofenceManagerImpl(IGeolocator geolocator = null, GeofenceSettings settings = null)
        {
            this.geolocator = geolocator ?? CrossGeolocator.Current;
            this.settings = settings ?? GeofenceSettings.GetInstance();
            this.states = new Dictionary<string, GeofenceState>();

            this.observe = Observable
                .Create<GeofenceStatusEvent>(ob =>
                {
                    var handler = new EventHandler<PositionEventArgs>((sender, args) =>
                    {
                        this.current = new Position(args.Position.Latitude, args.Position.Longitude);
                        this.UpdateFences(ob, args.Position.Latitude, args.Position.Longitude);
                    });
                    this.geolocator.PositionChanged += handler;
                    return () => this.geolocator.PositionChanged -= handler;
                })
                .Publish()
                .RefCount();
        }


        public IObservable<GeofenceStatusEvent> WhenRegionStatusChanged()
        {
            return this.observe;
        }


        public IReadOnlyList<GeofenceRegion> MonitoredRegions => this.settings.MonitoredRegions.ToList();
        public void StartMonitoring(GeofenceRegion region)
        {
            var state = new GeofenceState(region);
            if (this.current != null)
            {
                var inside = PositionUtils.IsInsideGeofence(region.Center, this.current, region.Radius);
                state.Status = inside ? GeofenceStatus.Entered : GeofenceStatus.Exited;
            }

            this.states.Add(region.Identifier, state);
            this.settings.MonitoredRegions.Add(region);

            if (!this.geolocator.IsListening)
                this.geolocator.StartListeningAsync(1, 10, false);
        }


        public void StopMonitoring(GeofenceRegion region)
        {
            this.states.Remove(region.Identifier);
            this.settings.MonitoredRegions.Remove(region);

            if (!this.settings.MonitoredRegions.Any())
                this.geolocator.StopListeningAsync();
        }


        public void StopAllMonitoring()
        {
            this.settings.MonitoredRegions.Clear();
            this.states.Clear();
        }


        void UpdateFences(IObserver<GeofenceStatusEvent> ob, double lat, double lng)
        {
            var loc = new Position(lat, lng);

            foreach (var fence in this.states.Values)
            {
                var newState = PositionUtils.IsInsideGeofence(fence.Region.Center, loc, fence.Region.Radius);
                var status = newState ? GeofenceStatus.Entered : GeofenceStatus.Exited;

                if (fence.Status == GeofenceStatus.Unknown)
                {
                    // status being set for first time as we didn't have current coordinates
                    fence.Status = status;
                }
                else
                {
                    fence.Status = status;
                    ob.OnNext(new GeofenceStatusEvent(fence.Region, status));
                }
            }
        }
    }
}