using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            this.Menu = new Command(() => dialogs.ActionSheet(new ActionSheetConfig()
                .SetCancel()
                .Add("Add Geofence", async () => 
                    await viewModelMgr.PushNav<AddGeofenceViewModel>()
                )
                .Add("Use Default Geofences", async () => 
                {
                    var result = await dialogs.ConfirmAsync(new ConfirmConfig()
                        .UseYesNo()
                        .SetMessage("This will stop monitoring all existing geofences and set the defaults.  Are you sure you want to do this?"));

                    if (result)
                    {
                        geofences.StopAllMonitoring();
                        await Task.Delay(500);
                        geofences.StartMonitoring(new GeofenceRegion
                        {
                            Identifier = "FC HQ",
                            Center = new Position(43.6411314, -79.3808415),   // 88 Queen's Quay - Home
                            Radius = Distance.FromKilometers(1)
                        });
                        geofences.StartMonitoring(new GeofenceRegion
                        {
                            Identifier = "Close to HQ",
                            Center = new Position(43.6411314, -79.3808415),   // 88 Queen's Quay
                            Radius = Distance.FromKilometers(3)
                        });
                        geofences.StartMonitoring(new GeofenceRegion 
                        {
                            Identifier = "Ajax GO Station",
                            Center = new Position(43.8477697, -79.0435461),
                            Radius = Distance.FromMeters(500)
                        });
                        await Task.Delay(500); // ios needs a second to breathe when registering like this
                        this.RaisePropertyChanged("CurrentRegions");
                    }                
                })
                .Add("Stop All Geofences", async () => 
                {
                    var result = await dialogs.ConfirmAsync(new ConfirmConfig()
                        .UseYesNo()
                        .SetMessage("Are you sure wish to stop monitoring all geofences?"));

                    if (result)
                    {
                        this.geofences.StopAllMonitoring();
                        this.RaisePropertyChanged("CurrentRegions");
                    }
                })
            ));
            
            this.Remove = new Command<GeofenceRegion>(x =>
            {
                this.geofences.StopMonitoring(x);
                this.RaisePropertyChanged("CurrentRegions");
            });
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


        public ICommand Menu { get; }
        public Command<GeofenceRegion> Remove { get; }

        [Reactive] public PermissionStatus GpsPermission { get; private set; } = PermissionStatus.Unknown;
        public IEnumerable<GeofenceViewModel> CurrentRegions => this.geofences
            .MonitoredRegions
            .Select(x => new GeofenceViewModel(x, new Command(() => 
            {
                this.geofences.StopMonitoring(x);
                this.RaisePropertyChanged("CurrentRegions");
            })));
    }
}
