using System;
using System.Collections.Generic;
using System.Linq;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;


namespace Acr.Geofencing
{
    public class GeofenceManagerImpl : IGeofenceManager
    {
        readonly IGeolocator geolocator;
        readonly GeofenceSettings settings;
        readonly IDictionary<string, GeofenceState> states;
        Position current;


        public GeofenceManagerImpl(IGeolocator geolocator = null, GeofenceSettings settings = null)
        {
            this.geolocator = geolocator ?? CrossGeolocator.Current;
            this.geolocator.AllowsBackgroundUpdates = true;
            this.geolocator.PositionChanged += (sender, args) =>
            {
                this.current = new Position(args.Position.Latitude, args.Position.Longitude);
                this.UpdateFences(args.Position.Latitude, args.Position.Longitude);
            };
            this.settings = settings ?? GeofenceSettings.GetInstance();
            this.DesiredAccuracy = Distance.FromMeters(200);

            this.states = new Dictionary<string, GeofenceState>();
            if (this.settings.MonitoredRegions.Count > 0)
                this.TryStartGeolocator();
        }


        public event EventHandler<GeofenceStatusChangedEventArgs> RegionStatusChanged;


        public Distance DesiredAccuracy
        {
            get
            {
                return Distance.FromMeters(this.geolocator.DesiredAccuracy);
            }
            set
            {
                this.geolocator.DesiredAccuracy = value.TotalMeters;
            }
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
            this.TryStartGeolocator();
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


        protected void TryStartGeolocator()
        {
            this.geolocator.AllowsBackgroundUpdates = true;
            if (!this.geolocator.IsListening)
                this.geolocator.StartListeningAsync(600, 200, false);
        }


        protected void UpdateFences(double lat, double lng)
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
                else if (fence.Status != status)
                {
                    fence.Status = status;
                    this.RegionStatusChanged?.Invoke(this, new GeofenceStatusChangedEventArgs(fence.Region, status));
                }
            }
        }
    }
}