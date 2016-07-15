using System;
using Autofac;
using Samples.Pages;
using Xamarin.Forms;


namespace Samples
{

    public class App : Application
    {
        readonly IContainer container;

        public App(IContainer container)
        {
            //var vm = null;
            this.MainPage = new HomePage
            {
                BindingContext = null
            };
        }


        //protected override async void OnStart() {
        //    var gi = Geofences.Instance;
        //    var result = await gi.Initialize();
        //    if (!result)
        //        return;

        //    gi.StopAllMonitoring();
        //    gi.StartMonitoring(new GeofenceRegion("angus", 1, 1, 400));
        //    gi.RegionStatusChanged += this.OnRegionStatusChanged;
        //}


        //private void OnRegionStatusChanged(object sender, GeofenceStatusChangedArgs e) {
        //    Data.Insert(new RegionEvent {
        //        Identifer = e.Region.Identifier,
        //        Status = e.Status,
        //        DateCreated = DateTime.Now
        //    });
        //}
    }
}
