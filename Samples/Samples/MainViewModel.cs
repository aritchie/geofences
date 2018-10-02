using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Plugin.Geofencing;
using Plugin.Permissions.Abstractions;
using Prism.AppModel;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Xamarin.Essentials;


namespace Samples
{
    public class MainViewModel : ReactiveObject, IPageLifecycleAware
    {
        const double DEFAULT_DISTANCE_METERS = 200;
        readonly IGeofenceManager geofenceManager;
        readonly IUserDialogs dialogs;
        readonly SqliteConnection conn;


        public MainViewModel(SqliteConnection conn,
                             IGeofenceManager geofenceManager,
                             IUserDialogs dialogs)
        {
            this.conn = conn;
            this.geofenceManager = geofenceManager;
            this.dialogs = dialogs;

            this.SetCurrentLocation = ReactiveCommand.CreateFromTask(async _ =>
            {
                var pos = await Geolocation.GetLastKnownLocationAsync();
                if (pos == null)
                    this.dialogs.Alert("Could not get current position");
                else
                {
                    this.Identifier = $"{pos.Latitude}_{pos.Longitude}";
                    this.CenterLatitude = pos.Latitude;
                    this.CenterLongitude = pos.Longitude;
                }
            });

            this.CreateGeofence = ReactiveCommand.CreateFromTask(
                async _ =>
                {
                    await this.AddGeofence(
                        this.Identifier,
                        this.CenterLatitude,
                        this.CenterLongitude,
                        this.RadiusMeters
                    );
                    this.Identifier = String.Empty;
                    this.CenterLatitude = 0;
                    this.CenterLongitude = 0;
                    this.RadiusMeters = DEFAULT_DISTANCE_METERS;
                },
                this.WhenAny(
                    x => x.Identifier,
                    x => x.RadiusMeters,
                    x => x.CenterLatitude,
                    x => x.CenterLongitude,
                    x => x.NotifyOnEntry,
                    x => x.NotifyOnExit,
                    (id, rad, lat, lng, entry, exit) =>
                    {
                        if (String.IsNullOrWhiteSpace(id.GetValue()))
                            return false;

                        var radius = rad.GetValue();
                        if (radius < 200 || radius > 5000)
                            return false;

                        var latv = lat.GetValue();
                        if (latv > 89.9 || latv < -89.9)
                            return false;

                        var lngv = lng.GetValue();
                        if (lngv > 179.9 || lngv < -179.9)
                            return false;

                        if (!entry.GetValue() && !exit.GetValue())
                            return false;

                        return true;
                    }
                )
            );

            this.DropAllFences = ReactiveCommand.CreateFromTask(
                async _ =>
                {
                    var confirm = await this.dialogs.ConfirmAsync("Are you sure you wish to drop all geofences?");
                    if (confirm)
                    {
                        this.geofenceManager.StopAllMonitoring();
                        this.LoadRegions();
                    }
                },
                this.WhenAny(
                    x => x.HasGeofences,
                    x => x.GetValue()
                )
            );


            var hasEventType = this.WhenAny(
                x => x.NotifyOnEntry,
                x => x.NotifyOnExit,
                (entry, exit) => entry.GetValue() || entry.GetValue()
            );

            this.AddCnTower = ReactiveCommand.CreateFromTask(
                _ => this.AddGeofence(
                    "CNTowerToronto",
                    43.6425662,
                    -79.3892508,
                    DEFAULT_DISTANCE_METERS
                ),
                hasEventType
            );

            this.AddAppleHQ = ReactiveCommand.CreateFromTask(
                _ => this.AddGeofence(
                    "AppleHQ",
                    37.3320045,
                    -122.0329699,
                    DEFAULT_DISTANCE_METERS
                ),
                hasEventType
            );
        }


        public ICommand CreateGeofence { get; }
        public ICommand DropAllFences { get; }
        public ICommand SetCurrentLocation { get; }

        public bool HasGeofences => this.Geofences.Any();
        public IList<GeofenceRegionViewModel> Geofences { get; private set; } = new List<GeofenceRegionViewModel>();
        public bool HasEvents => this.Events.Any();
        public IList<GeofenceEvent> Events { get; private set; } = new List<GeofenceEvent>();

        public ICommand AddCnTower { get; }
        public ICommand AddAppleHQ { get; }

        [Reactive] public string Identifier { get; set; }
        [Reactive] public double CenterLatitude { get; set; }
        [Reactive] public double CenterLongitude { get; set; }
        [Reactive] public double RadiusMeters { get; set; } = DEFAULT_DISTANCE_METERS;
        [Reactive] public bool SingleUse { get; set; }
        [Reactive] public bool NotifyOnEntry { get; set; } = true;
        [Reactive] public bool NotifyOnExit { get; set; } = true;


        public void OnAppearing()
        {
            this.LoadEvents();
            this.LoadRegions();

            this.geofenceManager.RegionStatusChanged += this.OnRegionStatusChanged;
        }


        public void OnDisappearing() => this.geofenceManager.RegionStatusChanged -= this.OnRegionStatusChanged;
        void OnRegionStatusChanged(object sender, GeofenceStatusChangedEventArgs args) => this.LoadEvents();


        async Task AddGeofence(string id, double lat, double lng, double distance)
        {
            var permission = await this.geofenceManager.RequestPermission();
            if (permission == PermissionStatus.Granted)
            {
                this.geofenceManager.StartMonitoring(new GeofenceRegion(
                    id,
                    new Position(lat, lng),
                    Distance.FromMeters(distance)
                )
                {
                    NotifyOnEntry = this.NotifyOnEntry,
                    NotifyOnExit = this.NotifyOnExit,
                    SingleUse = this.SingleUse
                });
                this.dialogs.Toast($"Geofence {id} Created");
                this.LoadRegions();
            }
            else
            {
                this.dialogs.Alert("Error getting geofence permission - " + permission);
            }
        }

        void LoadEvents()
        {
            this.Events = this.conn
                .GeofenceEvents
                .OrderBy(x => x.Date)
                .ToList();

            this.RaisePropertyChanged(nameof(this.Events));
            this.RaisePropertyChanged(nameof(this.HasEvents));
        }


        void LoadRegions()
        {
            this.Geofences = this.geofenceManager
                .MonitoredRegions
                .Select(region => new GeofenceRegionViewModel
                {
                    Region = region,
                    Remove = ReactiveCommand.CreateFromTask(async _ =>
                    {
                        var confirm = await this.dialogs.ConfirmAsync("Are you sure you wish to remove geofence - " + region.Identifier);
                        if (confirm)
                        {
                            this.geofenceManager.StopMonitoring(region);
                            this.LoadRegions();
                        }
                    }),
                    RequestCurrentState = ReactiveCommand.CreateFromTask(async _ =>
                    {
                        GeofenceStatus? status = null;
                        using (var cancelSrc = new CancellationTokenSource())
                        {
                            using (this.dialogs.Loading("Requesting State for " + region.Identifier, cancelSrc.Cancel))
                                status = await this.geofenceManager.RequestState(region, cancelSrc.Token);
                        }

                        if (status != null)
                        {
                            await Task.Delay(2000);
                            this.dialogs.Alert($"{region.Identifier} status is {status}");
                        }
                    })
                })
                .ToList();

            this.RaisePropertyChanged(nameof(this.Geofences));
            this.RaisePropertyChanged(nameof(this.HasGeofences));
        }
    }
}
