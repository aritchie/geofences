using System;


namespace Acr.Geofencing
{
    public static class PositionUtils
    {
        public static bool IsInsideGeofence(Position center, Position current, Distance radius)
        {
            var distance = center.GetDistanceTo(current);
            var inside = distance.TotalMeters <= radius.TotalMeters;
            return inside;
        }
    }
}
