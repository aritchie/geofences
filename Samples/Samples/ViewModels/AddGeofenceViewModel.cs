using System;
using System.Windows.Input;
using Acr.Geofencing;
using Acr.UserDialogs;


namespace Samples.ViewModels
{
    public class AddGeofenceViewModel : AbstractViewModel
    {
        public AddGeofenceViewModel(IGeofenceManager geofences, IUserDialogs dialogs)
        {

        }


        public ICommand Add { get; }
    }
}
