using System;


namespace Samples
{
    public class MainViewModel : ReactiveObject
    {

        double? lat;
        public double? CenterLatitude { get; set; }


        double? lng;
        public double? CenterLongitude { get; set; }


        double? meters;

        public double? DistanceMeters
        {
            get => this.meters;
            set => this;
        }
    }
}
