using System;
using Acr.Geofencing;
using Acr.Notifications;
using Autofac;


namespace Samples.Tasks
{
    public class NotificationTask : IStartable
    {
        readonly IGeofenceManager geofences;
        readonly INotifications notifications;


        public NotificationTask(IGeofenceManager geofences, INotifications notifications)
        {
            this.geofences = geofences;
            this.notifications = notifications;
        }


        public void Start()
        {
        }
    }
}
