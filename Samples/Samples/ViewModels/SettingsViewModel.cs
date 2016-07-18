using System;
using System.Collections.Generic;
using System.Windows.Input;
using Acr;
using Acr.Geofencing;
using Acr.UserDialogs;
using Plugin.Permissions.Abstractions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Samples.Services;


namespace Samples.ViewModels
{
    public class SettingsViewModel : AbstractViewModel
    {
        readonly IPermissions permissions;
        readonly IGeofenceManager geofences;
        readonly IUserDialogs dialogs;


        public SettingsViewModel(IPermissions permissions,
                                 IGeofenceManager geofences,
                                 IUserDialogs dialogs,
                                 IViewModelManager viewModelMgr)
        {
            this.permissions = permissions;
            this.geofences = geofences;
            this.dialogs = dialogs;

            this.Add = ReactiveCommand.CreateAsyncTask(async _ => await viewModelMgr.PushNav<AddGeofenceViewModel>());

            this.Remove = new Command<GeofenceRegion>(x =>
            {
                this.geofences.StopMonitoring(x);
                this.RaisePropertyChanged("CurrentRegions");
            });
            this.StopAll = ReactiveCommand.CreateAsyncTask(async x =>
            {
                var result = await dialogs.ConfirmAsync(new ConfirmConfig()
                    .UseYesNo()
                    .SetMessage("Are you sure wish to stop monitoring all geofences?"));

                if (result)
                {
                    this.geofences.StopAllMonitoring();
                    this.RaisePropertyChanged("CurrentRegions");
                }
            });
            this.ApplyDefaults = ReactiveCommand.CreateAsyncTask(
                async x =>
                {
                    var result = await dialogs.ConfirmAsync(new ConfirmConfig()
                        .UseYesNo()
                        .SetMessage("This will stop monitoring all existing geofences and set the defaults.  Are you sure you want to do this?"));

                    if (result)
                    {
                        geofences.StopAllMonitoring();
                        geofences.StartMonitoring(new GeofenceRegion
                        {
                            Identifier = "FC HQ",
                            Center = new Position(1, 1),   // 88 Queen's Quay - Home
                            Radius = Distance.FromKilometers(1)
                        });
                        geofences.StartMonitoring(new GeofenceRegion
                        {
                            Identifier = "Close to HQ",
                            Center = new Position(1, 1),   // 88 Queen's Quay
                            Radius = Distance.FromKilometers(3)
                        });
                        this.RaisePropertyChanged("CurrentRegions");
                    }
                }
            );
        }


        public override async void OnActivate()
        {
            base.OnActivate();
            var result = await this.permissions.RequestPermissionsAsync(Permission.Location);
            this.GpsPermission = result[Permission.Location];

            if (this.GpsPermission != PermissionStatus.Granted)
            {
                this.dialogs.Alert("Could not get location tracking permissions");
                this.RaisePropertyChanged("CurrentRegions");
            }
        }


        public ICommand Add { get; }
        public Command<GeofenceRegion> Remove { get; }
        public ICommand StopAll { get; }
        public ICommand ApplyDefaults { get; }

        [Reactive] public PermissionStatus GpsPermission { get; private set; } = PermissionStatus.Unknown;
        public IEnumerable<GeofenceRegion> CurrentRegions => this.geofences.MonitoredRegions;
    }
}
