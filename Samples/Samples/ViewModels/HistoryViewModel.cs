using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.Geofencing;
using ReactiveUI.Fody.Helpers;
using Samples.Models;


namespace Samples.ViewModels
{
    public class HistoryViewModel : AbstractViewModel
    {
        readonly SampleDbConnection conn;
        readonly IDisposable refresher;


        public HistoryViewModel(SampleDbConnection conn, IGeofenceManager geofences)
        {
            this.conn = conn;
            this.refresher = geofences
                .WhenRegionStatusChanged()
                .Subscribe(async x =>
                {
                    await Task.Delay(500); // let store task finish
                    this.Load();
                });
        }


        public override void OnActivate()
        {
            base.OnActivate();
            this.Load();
        }


        void Load()
        {
            this.IsLoading = true;
            this.Events = this.conn
                .GeofenceEvents
                .OrderBy(x => x.DateCreatedUtc)
                .ToList();
            this.IsLoading = false;
        }


        public ICommand Clear { get; }
        public ICommand Reload { get; }
        [Reactive] public bool IsLoading { get; private set; }
        [Reactive] public IList<GeofenceEvent> Events { get; private set; }
    }
}
