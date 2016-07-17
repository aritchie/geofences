using System;


namespace Acr.Geofencing
{
    internal class GeofenceState
    {
        public GeofenceState(GeofenceRegion region)
        {
            this.Region = region;
            this.Position = new Position(region.Latitude, region.Longitude);
            this.Status = GeofenceStatus.Unknown;
        }


        public GeofenceRegion Region { get; }
        public Position Position { get; }
        public GeofenceStatus Status { get; set; }
    }
}

