using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr;
using Acr.Geofencing;
using Acr.UserDialogs;
using Plugin.Messaging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Samples.Models;


namespace Samples.ViewModels
{
    public class HistoryViewModel : AbstractViewModel
    {
        readonly SampleDbConnection conn;
        readonly IGeofenceManager geofences;


        public HistoryViewModel(SampleDbConnection conn,
                                IGeofenceManager geofences,
                                IUserDialogs dialogs,
                                IMessaging messaging)
        {
            this.conn = conn;
            this.geofences = geofences;
            this.Reload = new Command(this.Load);

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

            this.SendDatabase = new Command(() =>
            {
                var backupLocation = conn.CreateDatabaseBackup(conn.Platform);

                var mail = new EmailMessageBuilder()
                    .Subject("Geofence Database")
                    //.WithAttachment(conn.DatabasePath, "application/octet-stream")
                    .WithAttachment(backupLocation, "application/octet-stream")
                    .Body("--")
                    .Build();

                messaging.EmailMessenger.SendEmail(mail);
            });
        }


        public override void OnActivate()
        {
            base.OnActivate();
            this.Load();
            this.geofences.RegionStatusChanged += this.OnRegionStatusChanged;
        }


        public override void OnDeactivate()
        {
            base.OnDeactivate();
            this.geofences.RegionStatusChanged -= this.OnRegionStatusChanged;
        }


        async void OnRegionStatusChanged(object sender, GeofenceStatusChangedEventArgs args)
        {
            await Task.Delay(500); // let task commit record first
            this.Load();
        }

        void Load()
        {
            this.IsLoading = true;
            this.Events = this.conn
                .GeofenceEvents
                .OrderByDescending(x => x.DateCreatedUtc)
                .Select(x => new GeofenceEventViewModel(x))
                .ToList();
            this.IsLoading = false;
        }


        public ICommand Reload { get; }
        public ICommand Clear { get; }
        public ICommand SendDatabase { get; }
        [Reactive] public bool IsLoading { get; private set; }
        [Reactive] public IList<GeofenceEventViewModel> Events { get; private set; }
    }
}
