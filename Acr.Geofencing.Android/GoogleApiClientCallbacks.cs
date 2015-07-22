using System;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.OS;


namespace Acr.Geofencing {

    public class GoogleApiClientCallbacks : Java.Lang.Object, IGoogleApiClientConnectionCallbacks, IGoogleApiClientOnConnectionFailedListener, IResultCallback {
        public event EventHandler<Bundle> Connected;
        public event EventHandler<ConnectionResult> ConnectionFailed;
        public event EventHandler<int> ConnectionSuspended;
        public event EventHandler<Java.Lang.Object> ResultReturned;


        public void OnConnected(Bundle connectionHint) {
            this.Connected?.Invoke(this, connectionHint);
        }


        public void OnConnectionFailed(ConnectionResult result) {
            this.ConnectionFailed?.Invoke(this, result);
        }


        public void OnConnectionSuspended(int cause) {
            this.ConnectionSuspended?.Invoke(this, cause);
        }


        public void OnResult(Java.Lang.Object obj) {
            this.ResultReturned?.Invoke(this, obj);
        }
    }
}