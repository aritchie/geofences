﻿using System;
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

[assembly: UsesPermission(Android.Manifest.Permission.AccessFineLocation)]
[assembly: UsesFeature("android.hardware.location.gps")]
[assembly: UsesFeature("android.hardware.location.network")]


namespace Plugin.Geofencing
{
    //https://developer.android.com/training/location/geofencing.html#java
    public class GeofenceManagerImpl : IGeofenceManager //Java.Lang.Object, IOnSuccessListener, IOnFailureListener
    {
        readonly PluginSqliteConnection conn;
        readonly GeofencingClient client;
        PendingIntent geofencePendingIntent;


        public GeofenceManagerImpl()
        {
            //CrossPermissions.Current.RequestPermissionsAsync(Permission.LocationAlways)
            this.conn = new PluginSqliteConnection();
            this.client = LocationServices.GetGeofencingClient(Application.Context);
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


        public IReadOnlyList<GeofenceRegion> MonitoredRegions => this.conn
            .GeofenceRegions
            .ToList()
            .Select(x => new GeofenceRegion(
                x.Identifier,
                new Position(x.CenterGpsLatitude, x.CenterGpsLongitude),
                Distance.FromMeters(x.RadiusMeters)
            ))
            .ToList();


        public async void StartMonitoring(GeofenceRegion region)
        {
            var geofence = new GeofenceBuilder()
                .SetRequestId(region.Identifier)
                .SetExpirationDuration(Geofence.NeverExpire)
                .SetCircularRegion(
                    region.Center.Latitude,
                    region.Center.Longitude,
                    Convert.ToSingle(region.Radius.TotalMeters)
                )
                .SetTransitionTypes(
                    Geofence.GeofenceTransitionEnter |
                    Geofence.GeofenceTransitionExit
                )
                .Build();

            var request = new GeofencingRequest.Builder()
                .AddGeofence(geofence)
                .SetInitialTrigger(GeofencingRequest.InitialTriggerEnter | GeofencingRequest.InitialTriggerExit)
                .Build();

            await this.client.AddGeofences(request, this.GetPendingIntent());
            this.conn.Insert(new DbGeofenceRegion
            {
                Identifier = region.Identifier,
                CenterGpsLatitude = region.Center.Latitude,
                CenterGpsLongitude = region.Center.Longitude,
                RadiusMeters = region.Radius.TotalMeters,
                DateCreatedUtc = DateTime.UtcNow
            });

                //if (this.regions.Count == 1)
                    Application.Context.StartService(new Intent(Application.Context, typeof(GeofenceIntentService)));
            //.AddOnSuccessListener(this)
            //.AddOnFailureListener(this);
        }


        public void StopMonitoring(GeofenceRegion region)
        {
            this.conn.Delete<DbGeofenceRegion>(region.Identifier);

            //lock (this.syncLock)
            //{
            //    this.client.RemoveGeofences(new List<string> { region.Identifier });
            //    if (this.regions.Remove(region))
            //        this.PersistRegions();

            //    if (this.regions.Count == 0)
            //        Application.Context.StopService(new Intent(Application.Context, typeof(GeofenceIntentService)));
            //}
        }


        public void StopAllMonitoring()
        {
            this.conn.DeleteAll<DbGeofenceRegion>();
            //    if (this.regions.Count == 0)
            //        return;

            //    lock (this.syncLock)
            //    {
            //        var ids = this.regions.Select(x => x.Identifier).ToList();
            //        this.client.RemoveGeofences(ids);
            //        this.regions.Clear();

            //        Application.Context.StopService(new Intent(Application.Context, typeof(GeofenceIntentService)));
            //    }
        }


        public Task<GeofenceStatus> RequestState(GeofenceRegion region, CancellationToken? cancelToken = null)
        {
            return Task.FromResult(GeofenceStatus.Unknown);
        }



        public event EventHandler<GeofenceStatusChangedEventArgs> RegionStatusChanged;


        //public void OnSuccess(Object result)
        //{
        //}


        //public void OnFailure(Exception e)
        //{
        //}


        protected virtual void OnRegionStatusChanged(GeofenceRegion region, GeofenceStatus status)
            => this.RegionStatusChanged?.Invoke(this, new GeofenceStatusChangedEventArgs(region, status));


        protected virtual PendingIntent GetPendingIntent()
        {
            if (this.geofencePendingIntent != null)
                return this.geofencePendingIntent;

            var intent = new Intent(Application.Context, typeof(GeofenceIntentService));
            this.geofencePendingIntent = PendingIntent.GetService(Application.Context, 0, intent, PendingIntentFlags.UpdateCurrent);

            return this.geofencePendingIntent;
        }


        //void PersistRegions() => this.settings.Set(nameof(this.MonitoredRegions), this.regions);
        //IList<GeofenceRegion> GetPersistedRegions() => this.settings.Get(nameof(this.MonitoredRegions), new List<GeofenceRegion>());


        internal void TryFireEvent(GeofencingEvent @event)
        {
            // TODO: I should track state internally so I can respond to RequestState
            if (this.RegionStatusChanged == null)
                return;

                var status = @event.GeofenceTransition == Geofence.GeofenceTransitionEnter
                    ? GeofenceStatus.Entered
                    : GeofenceStatus.Exited;

            //foreach (var native in @event.TriggeringGeofences)
            //{
            //    var region = this.regions.FirstOrDefault(x => x.Identifier.Equals(native.RequestId));
            //    if (region != null)
            //        this.OnRegionStatusChanged(region, status);
            //}
        }
    }
}
