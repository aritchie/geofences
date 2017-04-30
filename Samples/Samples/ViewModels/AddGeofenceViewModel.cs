using System;
using System.Windows.Input;
using Acr;
using Acr.Geofencing;
using Acr.UserDialogs;
using Plugin.Geolocator.Abstractions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Samples.Services;
using Position = Acr.Geofencing.Position;


namespace Samples.ViewModels
{
    public class AddGeofenceViewModel : AbstractViewModel
    {
        public AddGeofenceViewModel(IGeofenceManager geofences,
                                    IUserDialogs dialogs,
                                    IViewModelManager viewModelMgr,
                                    IGeolocator geolocator)
        {
            this.Add = ReactiveCommand.CreateFromTask(
                async x =>
                {
                    geofences.StartMonitoring(new GeofenceRegion
                    {
                        Identifier = this.Identifer,
                        Center = new Position(this.Latitude, this.Longitude),
                        Radius = Distance.FromMeters(this.RadiusMeters)
                    });
                    await viewModelMgr.PopNav();
                },
                this.WhenAny(
                    x => x.RadiusMeters,
                    x => x.Latitude,
                    x => x.Longitude,
                    x => x.Identifer,
                    (radius, lat, lng, id) =>
                    {
                        if (radius.Value < 100)
                            return false;

                        if (lat.Value < -90 || lat.Value > 90)
                            return false;

                        if (lng.Value < -180 || lng.Value > 180)
                            return false;

                        if (id.Value.IsEmpty())
                            return false;

                        return true;
                    }
                )
            );
            this.UseCurrentLocation = ReactiveCommand.CreateFromTask(async x =>
            {
                try
                {
                    var current = await geolocator.GetPositionAsync(5000);
                    this.Latitude = current.Latitude;
                    this.Longitude = current.Longitude;
                }
                catch
                {
                }
            });
        }


        public ICommand Add { get; }

        public ICommand UseCurrentLocation { get; }
        [Reactive] public double Latitude { get; set; }
        [Reactive] public double Longitude { get; set; }
        [Reactive] public int RadiusMeters { get; set; }
        [Reactive] public string Identifer { get; set; }
    }
}
