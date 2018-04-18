using System;
using Plugin.Geofencing;
using Xamarin.Forms;


namespace Samples
{
    public class App : Application
    {
        public static SqliteConnection Connection { get; } = new SqliteConnection();


        public App()
        {
            this.MainPage = new NavigationPage(new MainPage { Title = "Plugin.Geofencing" });
        }


        protected override void OnStart()
        {
            base.OnStart();
            CrossGeofences.Current.RegionStatusChanged += (sender, args) =>
            {
                Connection.Insert(new GeofenceEvent
                {
                    Identifier = args.Region.Identifier,
                    Date = DateTime.Now,
                    Entered = args.Status == GeofenceStatus.Entered
                });
            };
        }
    }
}
