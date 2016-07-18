using System;
using System.Collections.Generic;
using System.Windows.Input;
using Acr;
using Acr.Geofencing;
using Acr.UserDialogs;
using Plugin.Permissions.Abstractions;
using ReactiveUI;


namespace Samples.ViewModels
{
    public class SettingsViewModel : AbstractViewModel
    {
        readonly IPermissions permissions;
        readonly IGeofenceManager geofences;
        readonly IUserDialogs dialogs;


        public SettingsViewModel(IPermissions permissions,
                                 IGeofenceManager geofences,
                                 IUserDialogs dialogs)
        {
            this.permissions = permissions;
            this.geofences = geofences;
            this.dialogs = dialogs;

            this.Add = new Command(x =>
            {
            });
            this.Remove = new Command<GeofenceRegion>(x =>
            {
                this.geofences.StopMonitoring(x);
                this.RaisePropertyChanged("CurrentRegions");
            });
            this.StopAll = new Command(() => this.geofences.StopAllMonitoring());
        }


        public override async void OnActivate()
        {
            base.OnActivate();
            var result = await this.permissions.RequestPermissionsAsync(Permission.Location);
            if (result[Permission.Location] != PermissionStatus.Granted)
            {
                this.dialogs.Alert("Could not get location tracking permissions");
            }
            else
            {
                // TODO: if never monitored before, start with defaults
                // or have an add defaults button
                this.geofences.StartMonitoring(null);
            }
        }


        public ICommand Add { get; }
        public Command<GeofenceRegion> Remove { get; }
        public ICommand StopAll { get; }
        public IEnumerable<GeofenceRegion> CurrentRegions => this.geofences.MonitoredRegions;
    }
}
