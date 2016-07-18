using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.Geofencing;
using Acr.UserDialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Samples.Models;


namespace Samples.ViewModels
{
    public class HistoryViewModel : AbstractViewModel
    {
        readonly SampleDbConnection conn;
        readonly IDisposable refresher;


        public HistoryViewModel(SampleDbConnection conn, 
                                IGeofenceManager geofences,
                                IUserDialogs dialogs)
        {
            this.conn = conn;
            this.refresher = geofences
                .WhenRegionStatusChanged()
                .Subscribe(async x =>
                {
                    await Task.Delay(500); // let store task finish
                    this.Load();
                });
            this.Clear = ReactiveCommand.CreateAsyncTask(async x => 
            {
                var result = await dialogs.ConfirmAsync(new ConfirmConfig()
                   .UseYesNo()
                   .SetMessage("Are you sure you want to delete all of your history?"));

                if (result) 
                {
                    this.conn.DeleteAll<GeofenceEvent>();
                    this.Load();
                }
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
                .Select(x => new GeofenceEventViewModel(x))
                .ToList();
            this.IsLoading = false;
        }


        public ICommand Clear { get; }
        [Reactive] public bool IsLoading { get; private set; }
        [Reactive] public IList<GeofenceEventViewModel> Events { get; private set; }
    }
}
