using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            var list = this.conn.GeofenceRegions.ToList();
            foreach (var item in list)
            {
                var radius = Distance.FromMeters(item.CenterRadiusMeters);
                var center = new Position(item.CenterLatitude, item.CenterLongitude);
                var region = new GeofenceRegion(item.Identifier, center, radius);
                var state = new GeofenceState(region);
                this.states.Add(item.Identifier, state);
            }

            if (this.states.Count > 0)
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


        public IReadOnlyList<GeofenceRegion> MonitoredRegions => this.states.Values.Select(x => x.Region).ToList();


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
            var db = new DbGeofenceRegion
            {
                Identifier = region.Identifier,
                CenterLatitude = region.Center.Latitude,
                CenterLongitude = region.Center.Longitude,
                CenterRadiusMeters = region.Radius.TotalMeters
            };
            this.conn.Insert(db);
            this.states.Add(region.Identifier, new GeofenceState(region));

            this.TryStartGeolocator();
        }


        public void StopMonitoring(GeofenceRegion region)
        {
            this.states.Remove(region.Identifier);
            this.conn.GeofenceRegions.Delete(x => x.Identifier == region.Identifier);
            if (this.states.Any())
                this.StopGeolocator();
        }


        public void StopAllMonitoring()
        {
            this.states.Clear();
            this.conn.DeleteAll<DbGeofenceRegion>();
            this.StopGeolocator();
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