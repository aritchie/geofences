using System;
using System.Windows.Input;
using Acr.Geofencing;

namespace Samples.ViewModels
{
    public class GeofenceViewModel
    {
        readonly GeofenceRegion region;


        public GeofenceViewModel(GeofenceRegion region, ICommand removeCommand)
        {
            this.Region = region;
            this.Remove = removeCommand;
        }


        public GeofenceRegion Region { get; }
        public ICommand Remove { get; }
    }
}

