using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Acr.UserDialogs;
using Plugin.Geofencing;
using Plugin.Permissions.Abstractions;
using ReactiveUI;
using Xamarin.Forms;


namespace Samples
{
    public class MainViewModel : ReactiveObject
    {
        public MainViewModel()
        {
            this.Identifier = "CNTowerToronto";
            this.CenterLatitude = 43.6425662;
            this.CenterLongitude = -79.3892508;
            this.RadiusMeters = 200;


            this.CreateGeofence = ReactiveCommand.CreateFromTask(async _ =>
                {
                    try
                    {
                        var permission = await CrossGeofences.Current.RequestPermission();
                        if (permission == PermissionStatus.Granted)
                        {
                            CrossGeofences.Current.StartMonitoring(new GeofenceRegion(
                                this.Identifier,
                                new Position(this.CenterLatitude, this.CenterLongitude),
                                Distance.FromMeters(this.RadiusMeters)
                            ));
                            UserDialogs.Instance.Alert("Geofence Created");

                            this.CenterLatitude = 0;
                            this.CenterLongitude = 0;
                            this.RadiusMeters = 200;
                            this.Identifier = String.Empty;
                        }
                        else
                        {
                            UserDialogs.Instance.Alert("Error getting geofence permission - " + permission);
                        }

                        this.LoadRegions();
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

                        if (lat > 89.9 || lat < -89.9)
                            return false;

                        if (lng > 179.9 || lng < -179.9)
                            return false;

                        return true;
                    }
                )
            );

            this.DropFence = ReactiveCommand.Create<GeofenceRegion>(x =>
                UserDialogs.Instance.Confirm(new ConfirmConfig()
                    .UseYesNo()
                    .SetTitle("Confirm")
                    .SetMessage($"Are you sure you wish to stop monitoring {x.Identifier}?")
                    .SetAction(confirm =>
                    {
                        if (confirm)
                        {
                            CrossGeofences.Current.StopMonitoring(x);
                            this.LoadRegions();
                        }
                    })
                )
            );
        }


        public void Start()
        {
            this.LoadGeofences();
            this.LoadRegions();

            CrossGeofences.Current.RegionStatusChanged += (sender, args) => this.LoadGeofences();
        }


        void LoadGeofences()
        {
            this.Events = App
                .Connection
                .GeofenceEvents
                .OrderBy(x => x.Date)
                .ToList();

            this.RaisePropertyChanged(nameof(this.Events));
            this.RaisePropertyChanged(nameof(this.HasEvents));
        }


        void LoadRegions()
        {
            this.Geofences = CrossGeofences.Current.MonitoredRegions.Select(x => new GeofenceRegionViewModel(x)).ToList();
            this.RaisePropertyChanged(nameof(this.Geofences));
            this.RaisePropertyChanged(nameof(this.HasGeofences));
        }


        public ICommand CreateGeofence { get; }
        public ICommand DropFence { get; }
        public bool HasGeofences => this.Geofences.Any();
        public IList<GeofenceRegionViewModel> Geofences { get; private set; } = new List<GeofenceRegionViewModel>();
        public bool HasEvents => this.Events.Any();
        public IList<GeofenceEvent> Events { get; private set; } = new List<GeofenceEvent>();


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
