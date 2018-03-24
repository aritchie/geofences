using System;


namespace Plugin.Geofencing
{
    public static class PositionUtils
    {
        public static bool IsPositionInside(this GeofenceRegion region, Position position)
        {
            var distance = region.Center.GetDistanceTo(position);
            var inside = distance.TotalMeters <= region.Radius.TotalMeters;
            return inside;
        }
    }
}
