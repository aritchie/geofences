using System;
using GeoCoordinatePortable;


namespace Acr.Geofencing
{
    internal class GeofenceState
    {
        public GeofenceState(GeofenceRegion region) 
        {
            this.Region = region;
            this.Coordinate = new GeoCoordinate(region.Latitude, region.Longitude);
            this.Status = GeofenceStatus.Unknown;
        }


        public GeofenceRegion Region { get; }
        public GeoCoordinate Coordinate { get; }
        public GeofenceStatus Status { get; set; }
    }
}

