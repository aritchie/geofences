using System;


namespace Acr.Geofencing 
{
    public class GeofenceStatusEvent
    {

        public GeofenceRegion Region { get; }
        public GeofenceStatus Status { get; }


        public GeofenceStatusEvent(GeofenceRegion region, GeofenceStatus status) 
        {
            this.Region = region;
            this.Status = status;
        }
    }
}
