using System;
using Android.App;
using Android.Content;
using Android.Gms.Location;


namespace Plugin.Geofencing
{
    public class GeofenceIntentService : IntentService
    {
        protected override void OnHandleIntent(Intent intent)
        {
            var e = GeofencingEvent.FromIntent(intent);
            if (CrossGeofences.Current is GeofenceManagerImpl impl)
            {
                impl.TryFireEvent(e);
                return;
            }
            throw new ArgumentException("CrossGeofences.Current must be the GeofenceManagerImpl");
        }
    }
}
