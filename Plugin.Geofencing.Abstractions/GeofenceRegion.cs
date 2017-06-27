using System;


namespace Plugin.Geofencing
{
    public class GeofenceRegion
    {
        public string Identifier { get; set; }
        public Position Center { get; set; }
        public Distance Radius { get; set; }


        public override bool Equals(object obj) => this.Identifier.Equals(obj);
        public override int GetHashCode() => this.Identifier.GetHashCode();
    }
}
