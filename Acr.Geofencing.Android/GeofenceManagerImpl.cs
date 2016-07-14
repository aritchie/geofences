using System;
using GeoCoordinatePortable;


namespace Acr.Geofencing 
{

    public class GeofenceManagerImpl : AbstractGeofenceManagerImpl 
    {
        public GeofenceManagerImpl() 
        {
            // need observable geolocator - cake :)
        }


        public override IObservable<GeofenceStatusEvent> WhenRegionStatusChanged()
        {
            throw new NotImplementedException();
        }


        protected override void StartMonitoringNative(GeofenceRegion region)
        {
            throw new NotImplementedException();
        }


        protected override void StopMonitoringNative(GeofenceRegion region)
        {
            throw new NotImplementedException();
        }
    }
}