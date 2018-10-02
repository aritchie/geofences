using System;
using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Plugin.CurrentActivity;
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
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            UserDialogs.Init(() => (Activity)Forms.Context);
            Forms.Init(this, bundle);
            Xamarin.Essentials.Platform.Init(this, bundle);
            CrossCurrentActivity.Current.Init(this, bundle);
            this.LoadApplication(new App());
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
            => PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }
}