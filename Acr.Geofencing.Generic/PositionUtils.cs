using System;


namespace Acr.Geofencing
{
    public static class PositionUtils
    {
        public static bool IsInsideGeofence(this GeofenceRegion region, Position current)
        {
            var distance = region.Center.GetDistanceTo(current);
            var inside = distance.TotalMeters <= region.Radius.TotalMeters;
            return inside;
        }
    }
}
