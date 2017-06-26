using System;


namespace Plugin.Geofencing
{
    public class GeofenceStatusChangedEventArgs : EventArgs
    {
        public GeofenceStatusChangedEventArgs(GeofenceRegion region, GeofenceStatus status)
        {
            this.Region = region;
            this.Status = status;
        }


        public GeofenceRegion Region { get; }
        public GeofenceStatus Status { get; }
    }
}
