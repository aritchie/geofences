using System;


namespace Acr.Geofencing {

    public class GeofenceRegion {

        public GeofenceRegion() {
            this.StayThreshold = TimeSpan.FromMinutes(2);
        }


        public GeofenceRegion(string identifier, double latitude, double longitude, double radius) : this() {
            this.Identifier = identifier;
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.Radius = radius;
        }


        public string Identifier { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Radius { get; set; }
        public TimeSpan StayThreshold { get; set; }


        public override bool Equals(object obj) {
            return this.Identifier.Equals(obj);
        }


        public override int GetHashCode() {
            return this.Identifier.GetHashCode();
        }
    }
}
