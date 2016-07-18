using System;
using System.Windows.Input;
using Acr.Geofencing;
using Acr.UserDialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;


namespace Samples.ViewModels
{
    public class AddGeofenceViewModel : AbstractViewModel
    {
        public AddGeofenceViewModel(IGeofenceManager geofences, IUserDialogs dialogs)
        {
            this.Add = ReactiveCommand.CreateAsyncTask(
                async x =>
                {

                }
            );
        }


        [Reactive] public double Latitude { get; set; }
        [Reactive] public double Longitude { get; set; }
        [Reactive] public int RadiusKm { get; set; }
        [Reactive] public string Identifer { get; set; }
        public ICommand Add { get; }
    }
}
