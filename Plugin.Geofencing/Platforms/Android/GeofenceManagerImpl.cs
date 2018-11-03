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


namespace Plugin.Geofencing
{
    //https://developer.android.com/training/location/geofencing.html#java
    public class GeofenceManagerImpl : AbstractGeofenceManager //Java.Lang.Object, IOnSuccessListener, IOnFailureListener
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


        public override GeofenceManagerState Status => GeofenceManagerState.Ready;


        public override IReadOnlyList<GeofenceRegion> MonitoredRegions => this.current.Values
            .Select(x => new GeofenceRegion(
                x.Identifier,
                new Position(x.CenterGpsLatitude, x.CenterGpsLongitude),
                Distance.FromMeters(x.RadiusMeters)
             ))
            .ToList();


        public override async Task StartMonitoring(GeofenceRegion region)
        {
            await this.AssertPermission();

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


        public override async Task StopMonitoring(GeofenceRegion region)
        {
            await this.client.RemoveGeofences(new List<string> { region.Identifier });

            this.current.Remove(region.Identifier);
            this.conn.Delete<DbGeofenceRegion>(region.Identifier);

            if (this.current.Count == 0)
                Application.Context.StopService(new Intent(Application.Context, typeof(GeofenceIntentService)));
        }


        public override async Task StopAllMonitoring()
        {
            this.conn.DeleteAll<DbGeofenceRegion>();
            await this.client.RemoveGeofences(this.current.Keys.ToList());
            this.current.Clear();
            Application.Context.StopService(new Intent(Application.Context, typeof(GeofenceIntentService)));
        }


        public override async Task<GeofenceState> RequestState(GeofenceRegion region, CancellationToken cancelToken)
        {
            await this.AssertPermission();

            if (!this.current.ContainsKey(region.Identifier))
                return GeofenceState.Unknown;

            var result = (GeofenceState)this.current[region.Identifier].CurrentStatus;
            return result;
        }


        protected virtual void OnDbRegionStatusChanged(DbGeofenceRegion region, GeofenceState status)
        {
            var wrap = new GeofenceRegion(
                region.Identifier,
                new Position(region.CenterGpsLatitude, region.CenterGpsLongitude),
                Distance.FromMeters(region.RadiusMeters)
            )
            {
                NotifyOnEntry = region.NotifyOnEntry,
                NotifyOnExit = region.NotifyOnExit
            };
            this.OnGeofenceStatusChanged(wrap, status);
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
                ? GeofenceState.Entered
                : GeofenceState.Exited;

            foreach (var native in @event.TriggeringGeofences)
            {
                if (this.current.ContainsKey(native.RequestId))
                {
                    var wrap = this.current[native.RequestId];
                    wrap.CurrentStatus = (int)status;
                    this.conn.Update(wrap);

                    this.OnDbRegionStatusChanged(wrap, status);
                }
            }
        }
    }
}
