using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Acr.UserDialogs;
using Plugin.Geofencing;
using ReactiveUI;


namespace Samples
{
    public class MainViewModel : ReactiveObject
    {
        public MainViewModel()
        {
            // may want to manually add the event instead of spawning entire refresh (screen may not be active either)
            CrossGeofences.Current.RegionStatusChanged += (sender, args) => this.LoadEvents();

            this.AddFence = ReactiveCommand.Create(() =>
                {
                    try
                    {
                        CrossGeofences.Current.StartMonitoring(new GeofenceRegion(
                            this.Identifier,
                            new Position(this.CenterLatitude, this.CenterLongitude),
                            Distance.FromMeters(this.RadiusMeters)
                        ));
                        this.RaisePropertyChanged(nameof(this.MonitoredRegions));
                    }
                    catch (Exception ex)
                    {
                        UserDialogs.Instance.Alert(ex.ToString());
                    }
                },
                this.WhenAnyValue(
                    x => x.Identifier,
                    x => x.RadiusMeters,
                    x => x.CenterLatitude,
                    x => x.CenterLongitude,
                    (id, rad, lat, lng) =>
                    {
                        if (String.IsNullOrWhiteSpace(id))
                            return false;

                        if (rad < 200 || rad > 5000)
                            return false;

                        if (lat > 179.9 || lat < -179.9)
                            return false;

                        if (lng > 89.9 || lng < -89.9)
                            return false;

                        return true;
                    }
                )
            );

            this.DropFence = ReactiveCommand.Create<GeofenceRegion>(x =>
            {
                CrossGeofences.Current.StopMonitoring(x);
                this.RaisePropertyChanged(nameof(this.MonitoredRegions));
            });
        }


        void LoadEvents()
        {
            this.Events = App.Connection.GeofenceEvents.OrderBy(x => x.Date).ToList();
        }


        public ICommand AddFence { get; }
        public ICommand DropFence { get; }
        public IList<GeofenceRegion> MonitoredRegions => CrossGeofences.Current.MonitoredRegions.ToList();


        IList<GeofenceEvent> events;
        public IList<GeofenceEvent> Events
        {
            get => this.events;
            private set
            {
                this.events = value;
                this.RaisePropertyChanged();
            }
        }


        string identifier;
        public string Identifier
        {
            get => this.identifier;
            set => this.RaiseAndSetIfChanged(ref this.identifier, value);
        }


        double centerLatitude;
        public double CenterLatitude
        {
            get => this.centerLatitude;
            set => this.RaiseAndSetIfChanged(ref this.centerLatitude, value);
        }


        double centerLongitude;
        public double CenterLongitude
        {
            get => this.centerLongitude;
            set => this.RaiseAndSetIfChanged(ref this.centerLongitude, value);
        }


        double radius;
        public double RadiusMeters
        {
            get => this.radius;
            set => this.RaiseAndSetIfChanged(ref this.radius, value);
        }
    }
}
