using System;
using Acr.Geofencing;
using Samples.Models;


namespace Samples.ViewModels
{
    public class GeofenceEventViewModel
    {
        readonly GeofenceEvent @event;


        public GeofenceEventViewModel(GeofenceEvent @event)
        {
            this.@event = @event;
        }


        public string Identifier => this.@event.Identifier;
        public GeofenceStatus Status => this.@event.Status;
        public string Date => this.@event.DateCreatedUtc.ToLocalTime().ToString("U");
    }
}

