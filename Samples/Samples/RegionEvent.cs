using System;
using Acr.Geofencing;
using SQLite;


namespace Samples {

    public class RegionEvent {

        [PrimaryKey]
        public int Id { get; set; }
        public string Identifer { get; set; }
        public GeofenceStatus Status { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
