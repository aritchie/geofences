using System;


namespace Acr.Geofencing
{
    public class Position
    {
        public Position(double lat, double lng)
        {
            if (lat < -90 || lat > 90)
                throw new ArgumentException($"Invalid latitude value - {lat}");

            if (lng < -180 || lng > 180)
                throw new ArgumentException($"Invalid longitude value - {lng}");

            this.Latitude = lat;
            this.Longitude = lng;
        }


        public double Latitude { get; }
        public double Longitude { get; }


        // thanks geocoordinate
        public double GetDistanceTo(Position other)
        {
            var d1 = this.Latitude * (Math.PI / 180.0);
            var num1 = this.Longitude * (Math.PI / 180.0);
            var d2 = other.Latitude * (Math.PI / 180.0);
            var num2 = other.Longitude * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
                     Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

            return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
        }
    }
}
