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

            if (e.HasError)
            {

            }
            else if (e.GeofenceTransition == Geofence.GeofenceTransitionEnter)
            {

            }
            else if (e.GeofenceTransition == Geofence.GeofenceTransitionExit)
            {

            }
        }
    }
}
