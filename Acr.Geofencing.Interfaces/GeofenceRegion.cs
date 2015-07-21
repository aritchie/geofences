using System;


namespace Acr.Geofencing {

    public class GeofenceRegion {

        public GeofenceRegion() { }
        public GeofenceRegion(string identifier, double latitude, double longitude, double radius) {
            this.Identifier = identifier;
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.Radius = radius;
        }


        public string Identifier { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Radius { get; set; }
        public TimeSpan? StayThreshold { get; set; }


        public override bool Equals(object obj) {
            return base.Equals(obj);
        }


        public override int GetHashCode() {
            return this.Identifier.GetHashCode();
        }
    }
}
