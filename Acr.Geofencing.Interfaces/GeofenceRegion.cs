using System;


namespace Acr.Geofencing 
{
    public class GeofenceRegion 
    {
        public string Identifier { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Radius { get; set; }


        public override bool Equals(object obj) 
        {
            return this.Identifier.Equals(obj);
        }


        public override int GetHashCode() 
        {
            return this.Identifier.GetHashCode();
        }
    }
}
