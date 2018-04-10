using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Extensions;
using Android.Gms.Location;
using Android.Gms.Tasks;
using Exception = Java.Lang.Exception;
using Object = Java.Lang.Object;


namespace Plugin.Geofencing
{
    //https://developer.android.com/training/location/geofencing.html#java
    public class GeofenceManagerImpl : Java.Lang.Object, IGeofenceManager, IOnSuccessListener, IOnFailureListener
    {
        readonly GeofencingClient client;
        PendingIntent geofencePendingIntent;


        public GeofenceManagerImpl()
        {
            this.client = LocationServices.GetGeofencingClient(Application.Context);
        }


        public IReadOnlyList<GeofenceRegion> MonitoredRegions { get; }


        public async void StartMonitoring(GeofenceRegion region)
        {
            var geofence = new GeofenceBuilder()
                .SetRequestId(region.Identifier)
                .SetCircularRegion(
                    region.Center.Latitude,
                    region.Center.Longitude,
                    Convert.ToSingle(region.Radius.TotalMeters)
                )
                .SetTransitionTypes(Geofence.GeofenceTransitionEnter | Geofence.GeofenceTransitionExit)
                .Build();

            var request = new GeofencingRequest.Builder()
                .AddGeofence(geofence)
                .SetInitialTrigger(GeofencingRequest.InitialTriggerEnter)
                .Build();

            await this.client
                .AddGeofences(request, this.GetPendingIntent())
                .AddOnSuccessListener(this)
                .AddOnFailureListener(this);
        }


        public void StopMonitoring(GeofenceRegion region)
            => this.client.RemoveGeofences(new List<string> {region.Identifier});


        public void StopAllMonitoring()
        {
        }


        public Task<GeofenceStatus> RequestState(GeofenceRegion region, CancellationToken? cancelToken = null)
        {
            return System.Threading.Tasks.Task.FromResult(GeofenceStatus.Unknown);
        }


        public event EventHandler<GeofenceStatusChangedEventArgs> RegionStatusChanged;


        public void OnSuccess(Object result)
        {
        }


        public void OnFailure(Exception e)
        {
        }


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
    }
}
