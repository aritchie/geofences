using System;
using Acr.Geofencing;
using SQLite.Net.Attributes;


namespace Samples.Models
{
    public class GeofenceEvent
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }
        public DateTime DateCreatedUtc { get; set; }

        public string Identifier { get; set; }
        public GeofenceStatus Status { get; set; }
    }
}
