using System;


namespace Acr.Geofencing {

    public class GeofenceStatusChangedArgs : EventArgs {

        public GeofenceRegion Region { get; }
        public GeofenceStatus Status { get; }


        public GeofenceStatusChangedArgs(GeofenceRegion region, GeofenceStatus status) {
            this.Region = region;
            this.Status = status;
        }
    }
}
