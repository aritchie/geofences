using System;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using WinFence = Windows.Devices.Geolocation.Geofencing.GeofenceMonitor;


namespace Acr.Geofencing.Windows {

    public class GeofenceManagerImpl : AbstractGeofenceManagerImpl {

        public override async Task<bool> Initialize() {
            WinFence.Current.GeofenceStateChanged += this.OnGeofenceStateChanged;
            return true;
        }


        public override void StartMonitoring(GeofenceRegion region) {
            base.StartMonitoring(region);
            var position = new BasicGeoposition {
                Latitude = region.Latitude,
                Longitude = region.Longitude
            };
            var circle = new Geocircle(position, region.Radius);
            var fence = new Geofence(region.Identifier, circle, MonitoredGeofenceStates.Entered | MonitoredGeofenceStates.Exited, false, region.StayThreshold);
            WinFence.Current.Geofences.Add(fence);
        }


        public override void StopMonitoring(GeofenceRegion region) {
            base.StopMonitoring(region);

            //WinFence.Current.Geofences.Remove();
        }


        public override void StopAllMonitoring() {
            base.StopAllMonitoring();
            WinFence.Current.Geofences.Clear();
        }


        protected virtual void OnGeofenceStateChanged(WinFence sender, object args) {

        }
    }
}