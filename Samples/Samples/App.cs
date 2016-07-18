using System;
using System.Collections.Generic;
using Autofac;
using Samples.Services;
using Samples.ViewModels;
using Xamarin.Forms;


namespace Samples
{

    public class App : Application
    {
        readonly IContainer container;


        public App(IContainer container)
        {
            this.container = container;
            var vmMgr = this.container.Resolve<IViewModelManager>();

            this.MainPage = vmMgr.CreatePage<HomeViewModel>();
        }


        protected override void OnResume()
        {
            base.OnResume();
            var services = this.container.Resolve<IEnumerable<IAppLifecycle>>();
            foreach (var lifecycle in services)
                lifecycle.OnAppResume();
        }


        protected override void OnSleep()
        {
            base.OnSleep();
            var services = this.container.Resolve<IEnumerable<IAppLifecycle>>();
            foreach (var lifecycle in services)
                lifecycle.OnAppSleep();
        }
    }
}
