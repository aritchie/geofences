using System;


namespace Plugin.Geofencing
{
    public class GeofenceStatusChangedEventArgs : EventArgs
    {
        public GeofenceStatusChangedEventArgs(GeofenceRegion region, GeofenceState status)
        {
            this.Region = region;
            this.Status = status;
        }


        public GeofenceRegion Region { get; }
        public GeofenceState Status { get; }
    }
}
