using System;
using SQLite;


namespace Plugin.Geofencing.Platforms.Android
{
    public class DbGeofenceRegion
    {
        [PrimaryKey]
        public string Identifier { get; set; }

        public double RadiusMeters { get; set; }
        public double CenterGpsLatitude { get; set; }
        public double CenterGpsLongitude { get; set; }

        public int CurrentStatus { get; set; }
        public DateTime DateCreatedUtc { get; set; }
    }
}
