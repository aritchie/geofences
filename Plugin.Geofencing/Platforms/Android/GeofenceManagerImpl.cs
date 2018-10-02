using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Extensions;
using Android.Gms.Location;
using Plugin.Geofencing.Platforms.Android;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;


namespace Plugin.Geofencing
{
    //https://developer.android.com/training/location/geofencing.html#java
    public class GeofenceManagerImpl : IGeofenceManager //Java.Lang.Object, IOnSuccessListener, IOnFailureListener
    {
        readonly PluginSqliteConnection conn;
        readonly GeofencingClient client;
        readonly Dictionary<string, DbGeofenceRegion> current;
        PendingIntent geofencePendingIntent;


        public GeofenceManagerImpl()
        {
            this.client = LocationServices.GetGeofencingClient(Application.Context);
            this.conn = new PluginSqliteConnection();
            this.current = this.conn.GeofenceRegions.ToList().ToDictionary(
                x => x.Identifier,
                x => x
            );
        }


        public GeofenceManagerStatus Status => GeofenceManagerStatus.Ready;


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


        public IReadOnlyList<GeofenceRegion> MonitoredRegions => this.current.Values
            .Select(x => new GeofenceRegion(
                x.Identifier,
                new Position(x.CenterGpsLatitude, x.CenterGpsLongitude),
                Distance.FromMeters(x.RadiusMeters)
             ))
            .ToList();


        public async void StartMonitoring(GeofenceRegion region)
        {
            var transitions = this.GetTransitions(region);
            var geofence = new GeofenceBuilder()
                .SetRequestId(region.Identifier)
                .SetExpirationDuration(Geofence.NeverExpire)
                .SetCircularRegion(
                    region.Center.Latitude,
                    region.Center.Longitude,
                    Convert.ToSingle(region.Radius.TotalMeters)
                )
                .SetTransitionTypes(transitions)
                .Build();

            var request = new GeofencingRequest.Builder()
                .AddGeofence(geofence)
                .SetInitialTrigger(transitions)
                .Build();

            await this.client.AddGeofences(request, this.GetPendingIntent());

            var db = new DbGeofenceRegion
            {
                Identifier = region.Identifier,
                CenterGpsLatitude = region.Center.Latitude,
                CenterGpsLongitude = region.Center.Longitude,
                NotifyOnEntry = region.NotifyOnEntry,
                NotifyOnExit = region.NotifyOnExit,
                SingleUse = region.SingleUse,
                RadiusMeters = region.Radius.TotalMeters,
                DateCreatedUtc = DateTime.UtcNow
            };
            this.conn.Insert(db);
            this.current.Add(db.Identifier, db);

            if (this.current.Count == 1)
                Application.Context.StartService(new Intent(Application.Context, typeof(GeofenceIntentService)));
        }


        public void StopMonitoring(GeofenceRegion region)
        {
            this.client.RemoveGeofences(new List<string> { region.Identifier });

            this.current.Remove(region.Identifier);
            this.conn.Delete<DbGeofenceRegion>(region.Identifier);

            if (this.current.Count == 0)
                Application.Context.StopService(new Intent(Application.Context, typeof(GeofenceIntentService)));
        }


        public async void StopAllMonitoring()
        {
            this.conn.DeleteAll<DbGeofenceRegion>();
            await this.client.RemoveGeofences(this.current.Keys.ToList());
            this.current.Clear();
            Application.Context.StopService(new Intent(Application.Context, typeof(GeofenceIntentService)));
        }


        public Task<GeofenceStatus> RequestState(GeofenceRegion region, CancellationToken cancelToken)
        {
            if (!this.current.ContainsKey(region.Identifier))
                return Task.FromResult(GeofenceStatus.Unknown);

            var result = (GeofenceStatus)this.current[region.Identifier].CurrentStatus;
            return Task.FromResult(result);
        }


        public event EventHandler<GeofenceStatusChangedEventArgs> RegionStatusChanged;


        protected virtual void OnRegionStatusChanged(DbGeofenceRegion region, GeofenceStatus status)
        {
            if (this.RegionStatusChanged == null)
                return;

            var wrap = new GeofenceRegion(
                region.Identifier,
                new Position(region.CenterGpsLatitude, region.CenterGpsLongitude),
                Distance.FromMeters(region.RadiusMeters)
            )
            {
                NotifyOnEntry = region.NotifyOnEntry,
                NotifyOnExit = region.NotifyOnExit
            };
            this.RegionStatusChanged.Invoke(this, new GeofenceStatusChangedEventArgs(wrap, status));
        }


        protected virtual PendingIntent GetPendingIntent()
        {
            if (this.geofencePendingIntent != null)
                return this.geofencePendingIntent;

            var intent = new Intent(Application.Context, typeof(GeofenceIntentService));
            this.geofencePendingIntent = PendingIntent.GetService(Application.Context, 0, intent, PendingIntentFlags.UpdateCurrent);

            return this.geofencePendingIntent;
        }


        protected virtual int GetTransitions(GeofenceRegion region)
        {
            //GeofencingRequest.InitialTriggerEnter |
            //GeofencingRequest.InitialTriggerExit
            //    Geofence.GeofenceTransitionEnter |
            //    Geofence.GeofenceTransitionExit

            var i = 0;
            if (region.NotifyOnEntry)
                i = 1;

            if (region.NotifyOnExit)
                i += 2;

            return i;
        }


        internal void TryFireEvent(GeofencingEvent @event)
        {
            var status = @event.GeofenceTransition == Geofence.GeofenceTransitionEnter
                ? GeofenceStatus.Entered
                : GeofenceStatus.Exited;

            foreach (var native in @event.TriggeringGeofences)
            {
                if (this.current.ContainsKey(native.RequestId))
                {
                    var wrap = this.current[native.RequestId];
                    wrap.CurrentStatus = (int)status;
                    this.conn.Update(wrap);

                    this.OnRegionStatusChanged(wrap, status);
                }
            }
        }
    }
}
