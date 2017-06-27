using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;


namespace Plugin.Geofencing
{
    public class GeofenceManagerImpl : IGeofenceManager
    {
        readonly IGeolocator geolocator;
        readonly IDictionary<string, GeofenceState> states;
        readonly AcrSqliteConnection conn;
        Position current;
        DateTime? lastFix;


        public GeofenceManagerImpl(IGeolocator geolocator = null)
        {
            this.geolocator = geolocator ?? CrossGeolocator.Current;
            this.conn = new AcrSqliteConnection();
            this.DesiredAccuracy = Distance.FromMeters(200);

            this.states = new Dictionary<string, GeofenceState>();
            if (this.settings.MonitoredRegions.Count > 0)
                this.TryStartGeolocator();
        }


        public async Task<GeofenceStatus> RequestState(GeofenceRegion region, CancellationToken? cancelToken = null)
        {
            // TODO: what if my position is old?
            if (this.states.ContainsKey(region.Identifier))
                return this.states[region.Identifier].Status;

            if (this.current != null)
                return region.IsPositionInside(this.current) ? GeofenceStatus.Entered : GeofenceStatus.Exited;

            var tcs = new TaskCompletionSource<GeofenceStatus>();
            cancelToken?.Register(() => tcs.TrySetCanceled());

            var handler = new EventHandler<GeofenceStatusChangedEventArgs>((sender, args) =>
            {
                if (args.Region.Identifier.Equals(region.Identifier))
                    tcs.TrySetResult(args.Status);
            });

            try
            {
                // this is not ideal since it fires the public event, though they were likely monitoring it anyhow
                this.RegionStatusChanged += handler;
                this.StartMonitoring(region);
                return await tcs.Task;
            }
            finally
            {
                this.StopMonitoring(region);
                this.RegionStatusChanged -= handler;
            }
        }


        public event EventHandler<GeofenceStatusChangedEventArgs> RegionStatusChanged;


        public Distance DesiredAccuracy
        {
            get => Distance.FromMeters(this.geolocator.DesiredAccuracy);
            set => this.geolocator.DesiredAccuracy = value.TotalMeters;
        }


        public IReadOnlyList<GeofenceRegion> MonitoredRegions => this.settings.MonitoredRegions.ToList();
        public void StartMonitoring(GeofenceRegion region)
        {
            var state = new GeofenceState(region);
            if (this.current != null)
            {
                // TODO: what if my position is old?
                var inside = region.IsPositionInside(this.current);
                state.Status = inside ? GeofenceStatus.Entered : GeofenceStatus.Exited;
            }

            this.states.Add(region.Identifier, state);
            //this.settings.Add(region);
            this.TryStartGeolocator();
        }


        public void StopMonitoring(GeofenceRegion region)
        {
            this.states.Remove(region.Identifier);
            //this.settings.Remove(region);

            //if (!this.settings.MonitoredRegions.Any())
            //    this.StopGeolocator();
        }


        public void StopAllMonitoring()
        {
            this.states.Clear();
        }


        protected async Task<bool> TryStartGeolocator()
        {
            if (this.geolocator.IsListening)
                return false;

            this.geolocator.PositionChanged += this.OnPositionChanged;
            await this.geolocator.StartListeningAsync(TimeSpan.FromSeconds(30), 200, false);
            return true;
        }


        protected void StopGeolocator()
        {
            this.geolocator.StopListeningAsync();
            this.geolocator.PositionChanged -= this.OnPositionChanged;
        }


        protected void OnPositionChanged(object sender, PositionEventArgs args)
        {
            this.lastFix = DateTime.UtcNow;
            this.current = new Position(args.Position.Latitude, args.Position.Longitude);
            this.UpdateFences(args.Position.Latitude, args.Position.Longitude);
        }


        protected void UpdateFences(double lat, double lng)
        {
            var loc = new Position(lat, lng);

            foreach (var fence in this.states.Values)
            {
                var newState = fence.Region.IsPositionInside(loc);
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