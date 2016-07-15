using System;
using System.Collections.Generic;
using System.Windows.Input;
using Acr;
using Acr.Geofencing;
using Acr.UserDialogs;
using ReactiveUI;


namespace Samples.ViewModels
{
    public class SettingsViewModel : AbstractViewModel
    {
        readonly IGeofenceManager geofences;


        public SettingsViewModel(IGeofenceManager geofences, IUserDialogs dialogs)
        {
            this.geofences = geofences;

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


        public ICommand Add { get; }
        public Command<GeofenceRegion> Remove { get; }
        public ICommand StopAll { get; }
        public IEnumerable<GeofenceRegion> CurrentRegions => this.geofences.MonitoredRegions;
    }
}
