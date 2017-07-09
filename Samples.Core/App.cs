using System;
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

            var radius = new EntryCell { Label = "Radius" };
            radius.SetBinding(EntryCell.TextProperty, "Radius", converter: new DoubleConverter());

            var btn = new TextCell { Text = "Set Geofence" };
            btn.SetBinding(TextCell.CommandProperty, "SetGeofence");

            this.MainPage = new NavigationPage(new ContentPage
            {
                Title = "ACR Geofencing",
                BindingContext = new MainViewModel(),
                Content = new TableView
                {
                    Root = new TableRoot
                    {
                        new TableSection
                        {
                            current
                        },
                        new TableSection
                        {
                            lat,
                            lng,
                            radius
                        },
                        new TableSection
                        {
                            btn
                        }
                    }
                }
            });
        }


        protected override void OnStart()
        {
            base.OnStart();
            CrossGeofences.Current.RegionStatusChanged += (sender, args) =>
                CrossNotifications.Current.Send(new Notification
                {
                    Title = "Geofence Update",
                    Message = $"Geofence status for {args.Region.Identifier} changed to {args.Status}"
                });
        }
    }
}
