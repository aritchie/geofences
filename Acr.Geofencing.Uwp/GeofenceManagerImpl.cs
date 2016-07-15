using System;
using System.Collections.Generic;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using WinFence = Windows.Devices.Geolocation.Geofencing.GeofenceMonitor;


namespace Acr.Geofencing
{

    public class GeofenceManagerImpl : IGeofenceManager
    {
        public IObservable<GeofenceStatusEvent> WhenRegionStatusChanged()
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<GeofenceRegion> MonitoredRegions { get; }

        public void StartMonitoring(GeofenceRegion region)
        {
        }


        public void StopMonitoring(GeofenceRegion region)
        {
            var position = new BasicGeoposition
            {
                Latitude = region.Latitude,
                Longitude = region.Longitude
            };
            var circle = new Geocircle(position, region.Radius);
            var fence = new Geofence(
                region.Identifier,
                circle,
                MonitoredGeofenceStates.Entered | MonitoredGeofenceStates.Exited,
                false
            );
            WinFence.Current.Geofences.Add(fence);
            //WinFence.Current.Geofences.Remove();
        }


        public void StopAllMonitoring()
        {
            WinFence.Current.Geofences.Clear();
        }


        protected virtual void OnGeofenceStateChanged(WinFence sender, object args)
        {
        }
    }
}