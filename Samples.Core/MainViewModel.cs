using System;
using System.Linq;
using System.Windows.Input;
using Plugin.Geofencing;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using ReactiveUI;


namespace Samples
{
    public class MainViewModel : ReactiveObject
    {
        readonly IGeolocator gps = CrossGeolocator.Current;
        readonly IGeofenceManager geofences = CrossGeofences.Current;


        public MainViewModel()
        {
            var current = this.geofences.MonitoredRegions.FirstOrDefault();
            if (current != null)
            {
                this.CenterLatitude = current.Center.Latitude;
                this.CenterLongitude = current.Center.Longitude;
                this.DistanceMeters = current.Radius.TotalMeters;
            }

            this.RemoveAllGeofences = ReactiveCommand.Create(this.geofences.StopAllMonitoring);

            this.SetGeofence = ReactiveCommand.Create(() =>
            {
                this.geofences.StopAllMonitoring();
                this.geofences.StartMonitoring(new GeofenceRegion
                {
                    Identifier = "plugintest",
                    Radius = Distance.FromMeters(this.DistanceMeters.Value),
                    Center = new Plugin.Geofencing.Position(this.CenterLongitude.Value, this.CenterLatitude.Value)
                });
            },
            this.WhenAny(
                x => x.CenterLatitude,
                x => x.CenterLongitude,
                x => x.DistanceMeters,
                (latitude, longitude, dist) =>
                    latitude.Value > -180 &&
                    latitude.Value < 180 &&
                    longitude.Value > -90 &&
                    longitude.Value < 90 &&
                    dist.Value > 0 &&
                    dist.Value < 3000
            ));

            this.UseCurrentGps = ReactiveCommand.CreateFromTask(async ct =>
            {
                try
                {
                    var pos = await this.gps.GetPositionAsync(token: ct);
                    if (pos != null)
                    {
                        this.CenterLatitude = pos.Latitude;
                        this.CenterLongitude = pos.Longitude;
                    }
                }
                catch (Exception ex)
                {

                }
            });
        }


        public ICommand SetGeofence { get; }
        public ICommand UseCurrentGps { get; }
        public ICommand RemoveAllGeofences { get; }


        double? lat;
        public double? CenterLatitude
        {
            get => this.lat;
            set => this.RaiseAndSetIfChanged(ref this.lat, value);
        }


        double? lng;
        public double? CenterLongitude
        {
            get => this.lng;
            set => this.RaiseAndSetIfChanged(ref this.lng, value);
        }


        double? meters;
        public double? DistanceMeters
        {
            get => this.meters;
            set => this.RaiseAndSetIfChanged(ref this.meters, value);
        }
    }
}
