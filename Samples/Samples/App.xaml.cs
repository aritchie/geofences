using System;
using Autofac;
using Prism;
using Prism.Autofac;
using Prism.Ioc;
using Prism.Mvvm;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]


namespace Samples
{
    public partial class App : PrismApplication
    {
        public App() : this(null)  {}
        public App(IPlatformInitializer initializer) : base(initializer)
        {
            this.InitializeComponent();
        }


        protected override async void OnInitialized()
        {
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(viewType =>
            {
                var viewModelTypeName = viewType.FullName.Replace("Page", "ViewModel");
                var viewModelType = Type.GetType(viewModelTypeName);
                return viewModelType;
            });
            await this.NavigationService.NavigateAsync("NavigationPage/MainPage");
        }


        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage>();
        }


        protected override IContainerExtension CreateContainerExtension()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<CoreModule>();
            return new AutofacContainerExtension(builder);
        }
    }
}