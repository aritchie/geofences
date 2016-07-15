using System;
using Acr.Geofencing;
using Acr.Notifications;
using Acr.UserDialogs;
using Autofac;


namespace Samples
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder
                .RegisterAssemblyTypes(this.ThisAssembly)
                .Where(x => x.Namespace.StartsWith("Samples.ViewModels"))
                .AsSelf()
                .InstancePerDependency();

            builder
                .RegisterAssemblyTypes(this.ThisAssembly)
                .Where(x => x.Namespace.StartsWith("Samples.Tasks"))
                .AsImplementedInterfaces()
                .SingleInstance()
                .AutoActivate();

            builder
                .Register(x => Geofences.Instance)
                .As<IGeofenceManager>()
                .SingleInstance();

            builder
                .Register(x => Notifications.Instance)
                .As<INotifications>()
                .SingleInstance();

            builder
                .Register(x => UserDialogs.Instance)
                .As<IUserDialogs>()
                .SingleInstance();
        }
    }
}
