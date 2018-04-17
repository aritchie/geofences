using System;
using Plugin.Geofencing;


namespace Samples
{
    public class GeofenceRegionViewModel
    {
        public GeofenceRegionViewModel(GeofenceRegion region)
        {
            this.Region = region;
        }


        public GeofenceRegion Region { get; }
        public string Text => $"{this.Region.Identifier}";
        public string Detail => $"{this.Region.Radius.TotalMeters}m from {this.Region.Center.Latitude}/{this.Region.Center.Longitude}";
    }
}
