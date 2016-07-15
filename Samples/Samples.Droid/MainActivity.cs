using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Autofac;
using Acr.UserDialogs;
using Plugin.Permissions;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;


namespace Samples.Droid
{
    [Activity(
        Label = "Geofences",
        Icon = "@drawable/icon",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation
    )]
    public class MainActivity : FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Forms.Init(this, bundle);
            FormsAppCompatActivity.ToolbarResource = Resource.Layout.toolbar;
            FormsAppCompatActivity.TabLayoutResource = Resource.Layout.tabs;

            UserDialogs.Init(() => (Activity)Forms.Context);

            var builder = new ContainerBuilder();
            builder.RegisterModule(new PlatformModule());
            var container = builder.Build();

            this.LoadApplication(new App(container));
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}