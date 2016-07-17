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
                .Create<GeofenceStatusEvent>(async ob =>
                {
                    var handler = new EventHandler<PositionEventArgs>((sender, args) =>
                    {
                        this.current = new Position(args.Position.Latitude, args.Position.Longitude);
                        this.UpdateFences(ob, args.Position.Latitude, args.Position.Longitude);
                    });

                    this.geolocator.PositionChanged += handler;
                    await this.geolocator.StartListeningAsync(1, 10, false);
                    return () =>
                    {
                        this.geolocator.StopListeningAsync();
                        this.geolocator.PositionChanged -= handler;
                    };
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
                var inside = this.IsInsideFence(this.current.Latitude, this.current.Longitude, state);
                state.Status = inside ? GeofenceStatus.Entered : GeofenceStatus.Exited;
            }

            this.states.Add(region.Identifier, state);
            this.settings.MonitoredRegions.Add(region);
        }


        public void StopMonitoring(GeofenceRegion region)
        {
            this.states.Remove(region.Identifier);
            this.settings.MonitoredRegions.Remove(region);
        }


        public void StopAllMonitoring()
        {
            this.settings.MonitoredRegions.Clear();
        }


        bool IsInsideFence(double lat, double lng, GeofenceState state)
        {
            var distance = state.Position.GetDistanceTo(new Position(lat, lng));
            var inside = state.Region.Radius < distance;
            return inside;
        }


        void UpdateFences(IObserver<GeofenceStatusEvent> ob, double lat, double lng)
        {
            foreach (var fence in this.states.Values)
            {
                var newState = this.IsInsideFence(lat, lng, fence);
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


        //void AssertValidGeofenceRegion(GeofenceRegion region)
        //{
        //    if (String.IsNullOrWhiteSpace(region.Identifier))
        //        throw new ArgumentException("No identifier set");

        //    if (region.Latitude < -90 || region.Latitude > 90)
        //        throw new ArgumentException($"Invalid latitude value - {region.Latitude}");

        //    if (region.Longitude < -180 || region.Longitude > 180)
        //        throw new ArgumentException($"Invalid longitude value - {region.Longitude}");
        //}
    }
}