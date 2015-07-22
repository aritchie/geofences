using System;
using Acr.Geofencing;
using SQLite;
using Xamarin.Forms;


namespace Samples {

    public class App : Application {
        public static SQLiteConnection Data { get; } = new SQLiteConnection("test.db");


        public App() {
            this.MainPage = new NavigationPage(new ListPage());
            Data.CreateTable<RegionEvent>();
        }


        protected override async void OnStart() {
            var gi = Geofences.Instance;
            var result = await gi.Initialize();
            if (!result)
                return;

            gi.StopAllMonitoring();
            gi.StartMonitoring(new GeofenceRegion("angus", 1, 1, 400));
            gi.RegionStatusChanged += this.OnRegionStatusChanged;
        }


        private void OnRegionStatusChanged(object sender, GeofenceStatusChangedArgs e) {
            Data.Insert(new RegionEvent {
                Identifer = e.Region.Identifier,
                Status = e.Status,
                DateCreated = DateTime.Now
            });
        }
    }
}
