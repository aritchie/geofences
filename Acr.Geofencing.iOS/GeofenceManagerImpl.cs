using System;
using System.Threading.Tasks;
using CoreLocation;
using Foundation;
using UIKit;


namespace Acr.Geofencing {

    public class GeofenceManagerImpl : AbstractGeofenceManagerImpl {

        public override async Task<bool> Initialize() {
            if (!UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
                return false;

			var good = false;
			//var authStatus = BeaconManager.AuthorizationStatus();

			if (authStatus != CLAuthorizationStatus.NotDetermined)
				good = this.IsGoodStatus(authStatus);

			else {
				var tcs = new TaskCompletionSource<bool>();
				//var funcPnt = new EventHandler<AuthorizationStatusChangedArgsEventArgs>((sender, args) => {
				//	if (args.Status == CLAuthorizationStatus.NotDetermined)
				//		return; // not done yet

				//	var status = this.IsGoodStatus(args.Status);
				//	tcs.TrySetResult(status);
				//});
				//this.beaconManager.AuthorizationStatusChanged += funcPnt;
				//this.beaconManager.RequestAlwaysAuthorization();
				good = await tcs.Task;
				//this.beaconManager.AuthorizationStatusChanged -= funcPnt;
			}
			return good;
        }


		private bool IsGoodStatus(CLAuthorizationStatus status) {
			return (
			    status == CLAuthorizationStatus.Authorized ||
			    status == CLAuthorizationStatus.AuthorizedAlways ||
			    status == CLAuthorizationStatus.AuthorizedWhenInUse
			);
		}
    }
}