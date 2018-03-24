using System;
using Acr.UserDialogs;
using Plugin.Geofencing;
using Plugin.Notifications;
using Xamarin.Forms;


namespace Samples
{
    public class App : Application
    {
        public App()
        {
            var current = new TextCell { Text = "Use Current GPS Coords" };
            current.SetBinding(TextCell.CommandProperty, "UseCurrentGps");

            var lat = new EntryCell { Label = "Center Lat"};
            lat.SetBinding(EntryCell.TextProperty, "CenterLatitude", converter: new DoubleConverter());

            var lng = new EntryCell { Label = "Center Lng" };
            lng.SetBinding(EntryCell.TextProperty, "CenterLongitude", converter: new DoubleConverter());

            var radius = new EntryCell { Label = "Radius (meters)" };
            radius.SetBinding(EntryCell.TextProperty, "DistanceMeters", converter: new DoubleConverter());

            var btn = new TextCell { Text = "Set Geofence" };
            btn.SetBinding(TextCell.CommandProperty, "SetGeofence");

            var btnStop = new TextCell { Text = "Stop Geofence" };
            btnStop.SetBinding(TextCell.CommandProperty, "StopGeofence");

            var btnStatus = new TextCell { Text = "Request Current Status" };
            btnStatus.SetBinding(TextCell.CommandProperty, "RequestStatus");

            this.MainPage = new NavigationPage(new ContentPage
            {
                Title = "ACR Geofencing",
                BindingContext = new MainViewModel(),
                Content = new TableView
                {
                    Root = new TableRoot
                    {
                        new TableSection("Details")
                        {
                            lat,
                            lng,
                            radius
                        },
                        new TableSection("Actions")
                        {
                            current,
                            btn,
                            btnStop,
                            btnStatus
                        }
                    }
                }
            });
        }


        protected override void OnStart()
        {
            base.OnStart();
            CrossGeofences.Current.RegionStatusChanged += (sender, args) =>
            {
                //var msg = $"Geofence status for {args.Region.Identifier} changed to {args.Status}";
                //CrossNotifications.Current.Send(new Notification
                //{
                //    Title = "Geofence Update",
                //    Message = msg
                //});
                //try {
                //    UserDialogs.Instance.Alert(msg, "Geofence Update");
                //}
                //catch {} // catch and release
            };
        }
    }
}
