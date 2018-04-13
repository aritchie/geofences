using System;
using SQLite;


namespace Samples
{
    public class GeofenceEvent
    {
        [PrimaryKey]
        public int Id { get; set; }

        public bool Entered { get; set; }
        public string Identifier { get; set; }
        public DateTime Date { get; set; }
    }
}
