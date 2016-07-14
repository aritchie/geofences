using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Locations;
using Android.OS;


namespace Acr.Geofencing {

    public class GeofenceManagerImpl : AbstractGeofenceManagerImpl {
		private readonly object syncLock = new object();

        private readonly LocationManager locationManager;
        private readonly GoogleApiClientCallbacks callbacks;


        public GeofenceManagerImpl() {
            this.callbacks = new GoogleApiClientCallbacks();
            this.locationManager = (LocationManager)Application.Context.GetSystemService(Context.LocationService);
            this.googleApi = new GoogleApiClientBuilder(Application.Context)
                .AddApi(LocationServices.Api)
                .AddConnectionCallbacks(this.callbacks)
                .AddOnConnectionFailedListener(this.callbacks)
                .Build();
        }


        public override async Task<bool> Initialize() {
            if (this.googleApi.IsConnected)
                return true;

            if (!this.locationManager.IsProviderEnabled(LocationManager.GpsProvider))
                return false;

            var queryResult = GooglePlayServicesUtil.IsGooglePlayServicesAvailable(Application.Context);
            if (queryResult == ConnectionResult.Success)
                return false;

            var tcs = new TaskCompletionSource<bool>();
            EventHandler<Bundle> connected = null;
            EventHandler<ConnectionResult> failed = null;

			lock (this.syncLock) {
				if (this.isConnected)
					tcs.TrySetResult(true);

                connected = (sender, args) => tcs.TrySetResult(true);
                failed = (sender, args) => tcs.TrySetResult(false);
                this.callbacks.Connected += connected;
                this.callbacks.ConnectionFailed += failed;
                this.googleApi.Connect();
            }

            await tcs.Task;
            if (connected != null) {
                this.callbacks.Connected -= connected;
                this.callbacks.ConnectionFailed -= failed;
            }
            return this.googleApi.IsConnected;
        }


        public override void StartMonitoring(GeofenceRegion region) {
            base.StartMonitoring(region);
            var native = new GeofenceBuilder()
                .SetRequestId(region.Identifier)
                .SetCircularRegion(region.Latitude, region.Longitude, (float)region.Radius)
                .SetLoiteringDelay((int)region.StayThreshold.TotalMilliseconds)
                .SetExpirationDuration(Geofence.NeverExpire)
                .SetTransitionTypes(Geofence.GeofenceTransitionEnter | Geofence.GeofenceTransitionExit | Geofence.GeofenceTransitionDwell)
                .Build();

            var request = new GeofencingRequest
                .Builder()
                .SetInitialTrigger(GeofencingRequest.InitialTriggerEnter)
                .AddGeofence(native)
                .Build();

            //LocationServices.GeofencingApi.AddGeofences(null, request, GeofencingApi.AddGeofences).SetResultCallback();
        }


        public override void StopMonitoring(GeofenceRegion region) {
            base.StopMonitoring(region);
        }
    }
}