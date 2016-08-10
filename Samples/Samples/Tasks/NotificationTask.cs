using System;
using Acr.Geofencing;
using Acr.Notifications;
using Autofac;


namespace Samples.Tasks
{
    public class NotificationTask : IStartable, IAppLifecycle
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
            this.geofences.RegionStatusChanged += (sender, args) =>
            {
                var exit = args.Status == GeofenceStatus.Exited;

                var notification = new Notification
                {
                    Title = exit ? "Bye" : "Welcome",
                    Message = exit
                        ? $"You have left the region \"{args.Region.Identifier}\""
                        : $"You have entered the region \"{args.Region.Identifier}\""
                };
                this.notifications.Send(notification);
                this.notifications.Badge = this.notifications.Badge + 1;
            };
        }


        public void OnAppResume()
        {
            this.notifications.Badge = 0;
        }


        public void OnAppSleep()
        {
        }
    }
}
