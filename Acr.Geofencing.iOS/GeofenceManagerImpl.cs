using System;
using System.Threading.Tasks;
using CoreLocation;
using UIKit;


namespace Acr.Geofencing {

    public class GeofenceManagerImpl : AbstractGeofenceManagerImpl {
        private readonly CLLocationManager locationManager;


        public GeofenceManagerImpl() {
            this.locationManager = new CLLocationManager();
            this.locationManager.RegionEntered += (sender, args) => {
                var native = args.Region as CLCircularRegion;
                if (native == null)
                    return;

                var region = this.FromNative(native);
                this.OnRegionStatusChanged(region, GeofenceStatus.Entered);
            };
            this.locationManager.RegionLeft += (sender, args) => {
                var native = args.Region as CLCircularRegion;
                if (native == null)
                    return;

                var region = this.FromNative(native);
                // TODO: stay duration?
                this.OnRegionStatusChanged(region, GeofenceStatus.Exited);
            };
            //this.locationManager.DesiredAccuracy = CLLocation.AccuracyNearestTenMeters; // TODO: configurable
        }


        public override async Task<bool> Initialize() {
            if (!UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
                return false;

			var good = false;
			var authStatus = CLLocationManager.Status;

			if (authStatus != CLAuthorizationStatus.NotDetermined)
				good = this.IsGoodStatus(authStatus);

			else {
				var tcs = new TaskCompletionSource<bool>();
				var funcPnt = new EventHandler<CLAuthorizationChangedEventArgs>((sender, args) => {
					if (args.Status == CLAuthorizationStatus.NotDetermined)
						return; // not done yet

					var status = this.IsGoodStatus(args.Status);
					tcs.TrySetResult(status);
			    });
				this.locationManager.AuthorizationChanged += funcPnt;
				this.locationManager.RequestAlwaysAuthorization();
				good = await tcs.Task;
				this.locationManager.AuthorizationChanged -= funcPnt;
			}
			return good;
        }


        public override void StartMonitoring(GeofenceRegion region) {
            var native = this.ToNative(region);
            this.locationManager.StartMonitoring(native); // TODO: accuracy?
            base.StartMonitoring(region);
        }


        public override void StopMonitoring(GeofenceRegion region) {
            var native = this.ToNative(region);
            this.locationManager.StartMonitoring(native);
            base.StopMonitoring(region);
        }


        protected virtual GeofenceRegion FromNative(CLCircularRegion native) {
            return new GeofenceRegion(
                native.Identifier,
                native.Center.Latitude,
                native.Center.Longitude,
                native.Radius
            );
        }


        protected virtual CLCircularRegion ToNative(GeofenceRegion region) {
            var center = new CLLocationCoordinate2D(region.Latitude, region.Longitude);
            return new CLCircularRegion(center, region.Radius, region.Identifier);
        }


        protected virtual bool IsGoodStatus(CLAuthorizationStatus status) {
			return (
			    status == CLAuthorizationStatus.Authorized ||
			    status == CLAuthorizationStatus.AuthorizedAlways ||
			    status == CLAuthorizationStatus.AuthorizedWhenInUse
			);
		}
    }
}