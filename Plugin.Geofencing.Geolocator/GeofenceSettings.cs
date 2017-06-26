using System;
using System.Collections.Generic;


namespace Plugin.Geofencing
{
    public static class GeofenceSettings
    {
        public static IList<GeofenceRegion> MonitoredRegions { get; set; } = new List<GeofenceRegion>();


        public static void Add(GeofenceRegion region)
        {
            MonitoredRegions.Add(region);
        }


        public static void Remove(GeofenceRegion region)
        {
            MonitoredRegions.Remove(region);
        }
    }
}