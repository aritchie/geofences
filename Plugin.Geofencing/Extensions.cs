using System;


namespace Plugin.Geofencing
{
    public static class Extensions
    {
        public static void StopMonitoring(this IGeofenceManager geofenceMgr, string identifier)
        {
            foreach (var region in geofenceMgr.MonitoredRegions)
            {
                if (region.Identifier.Equals(identifier, StringComparison.InvariantCultureIgnoreCase))
                {
                    geofenceMgr.StopMonitoring(region);
                    break;
                }
            }
        }


        public static bool IsPositionInside(this GeofenceRegion region, Position position)
        {
            var distance = region.Center.GetDistanceTo(position);
            var inside = distance.TotalMeters <= region.Radius.TotalMeters;
            return inside;
        }
    }
}
