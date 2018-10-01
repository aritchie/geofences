using System;
using SQLite;


namespace Plugin.Geofencing
{
    [Table("GeofenceRegions")]
    public class DbGeofenceRegion
    {
        [PrimaryKey]
        public string Identifier { get; set; }

        public double RadiusMeters { get; set; }
        public double CenterGpsLatitude { get; set; }
        public double CenterGpsLongitude { get; set; }
        public bool SingleUse { get; set; }
        public bool NotifyOnEntry { get; set; }
        public bool NotifyOnExit { get; set; }

        public int CurrentStatus { get; set; }
        public DateTime DateCreatedUtc { get; set; }
    }
}
