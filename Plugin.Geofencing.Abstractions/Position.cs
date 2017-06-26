using System;


namespace Plugin.Geofencing
{
    public class Position
    {
        public Position() { }
        public Position(double lat, double lng)
        {
            this.Latitude = lat;
            this.Longitude = lng;
        }


        double latitude;
        public double Latitude
        {
            get { return this.latitude; }
            set
            {
                if (value < -90 || value > 90)
                    throw new ArgumentException($"Invalid latitude value - {value}");

                this.latitude = value;
            }
        }


        double longitude;
        public double Longitude
        {
            get { return this.longitude; }
            set
            {
                if (value < -180 || value > 180)
                    throw new ArgumentException($"Invalid longitude value - {value}");

                this.longitude = value;
            }
        }



        public Distance GetDistanceTo(Position other)
        {
            var d1 = this.Latitude * (Math.PI / 180.0);
            var num1 = this.Longitude * (Math.PI / 180.0);
            var d2 = other.Latitude * (Math.PI / 180.0);
            var num2 = other.Longitude * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
                     Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

            var meters = 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
            return Distance.FromMeters(meters);
        }


        public override string ToString()
        {
            return $"Latitude: {this.Latitude} - Longitude: {this.Longitude}";
        }
    }
}
