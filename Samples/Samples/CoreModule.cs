using System;
using Acr.UserDialogs;
using Autofac;
using Plugin.Geofencing;


namespace Samples
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<SqliteConnection>().AsSelf().SingleInstance();
            builder.RegisterType<GeofenceLoggingTask>().As<IStartable>().AutoActivate().SingleInstance();
            builder.Register(_ => UserDialogs.Instance).As<IUserDialogs>().SingleInstance();
            builder.Register(_ => CrossGeofences.Current).As<IGeofenceManager>().SingleInstance();
            builder.RegisterType<GlobalExceptionHandler>().As<IStartable>().AutoActivate().SingleInstance();
        }
    }
}
