using System;
using System.Collections.Generic;
using System.Linq;
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


        protected override void OnResume()
        {
            base.OnResume();
            foreach (var lifecycle in this.container.Resolve<IEnumerable<IAppLifecycle>>())
                lifecycle.OnAppResume();
        }


        protected override void OnSleep()
        {
            base.OnSleep();
            foreach (var lifecycle in this.container.Resolve<IEnumerable<IAppLifecycle>>())
                lifecycle.OnAppSleep();
        }
    }
}
