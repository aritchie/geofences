using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;


namespace Plugin.Geofencing
{
    //https://docs.microsoft.com/en-us/windows/uwp/maps-and-location/set-up-a-geofence
    public class GeofenceManagerImpl : IGeofenceManager
    {
        public GeofenceManagerImpl()
        {
            GeofenceMonitor.Current.GeofenceStateChanged += (sender, args) =>
            {
            };
            GeofenceMonitor.Current.StatusChanged += (sender, args) => { };
        }


        public IReadOnlyList<GeofenceRegion> MonitoredRegions { get; }

        //<Capabilities>
        //<!-- DeviceCapability elements must follow Capability elements(if present) -->
        //<DeviceCapability Name = "location" />
        //    </ Capabilities >
        public async void StartMonitoring(GeofenceRegion region)
        {
            var accessStatus = await Geolocator.RequestAccessAsync();
            //string fenceId = "fence1";

            //// Define the fence location and radius.
            //BasicGeoposition position;
            //position.Latitude = 47.6510;
            //position.Longitude = -122.3473;
            //position.Altitude = 0.0;
            //double radius = 10; // in meters

            //// Set a circular region for the geofence.
            //Geocircle geocircle = new Geocircle(position, radius);

            //// Create the geofence.
            //Geofence geofence = new Geofence(fenceId, geocircle);
        }

        public void StopMonitoring(GeofenceRegion region)
        {
            throw new NotImplementedException();
        }

        public void StopAllMonitoring()
        {
            throw new NotImplementedException();
        }

        public Distance DesiredAccuracy { get; set; }
        public Task<GeofenceStatus> RequestState(GeofenceRegion region, CancellationToken? cancelToken = null)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<GeofenceStatusChangedEventArgs> RegionStatusChanged;
    }
}
