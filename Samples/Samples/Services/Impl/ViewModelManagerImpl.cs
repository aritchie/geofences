using System;
using System.Threading.Tasks;
using Autofac;
using Samples.ViewModels;
using Xamarin.Forms;


namespace Samples.Services.Impl
{
    public class ViewModelManagerImpl : IViewModelManager
    {
        readonly ILifetimeScope scope;


        public ViewModelManagerImpl(ILifetimeScope scope)
        {
            this.scope = scope;
        }


        public TViewModel Create<TViewModel>(object args = null) where TViewModel : class, IViewModel
        {
            var vm = this.scope.Resolve<TViewModel>();
            vm.Init(args);
            return vm;
        }


        public Task PushNav<TViewModel>(object args = null) where TViewModel : class, IViewModel
        {
			var page = this.CreatePage<TViewModel>(args);
			return GetDetailNav().PushAsync(page);
        }


        public Task PopNav()
        {
			return GetDetailNav().PopAsync(true);
        }



		public Page CreatePage<TViewModel>(object args) where TViewModel : class, IViewModel
		{
			var viewModel = this.Create<TViewModel>(args);
			var pageTypeName = viewModel
				.GetType()
				.FullName
				.Replace("ViewModel", "Page");

			var pageType = Type.GetType(pageTypeName);
			if (pageType == null)
				throw new ArgumentException("No corresponding page found for viewmodel");

			var page = Activator.CreateInstance(pageType) as Page;
			//var page = this.scope.Resolve(pageType) as Page;
			if (page == null)
				throw new ArgumentException("No page resolved for " + pageTypeName);

			page.BindingContext = viewModel;
			return page;
		}


		static INavigation GetDetailNav()
		{
			var tab = Application.Current.MainPage as TabbedPage;
			if (tab == null)
				throw new ArgumentException("Root page is not a TabbedPage");

			return tab.CurrentPage.Navigation;
		}
    }
}
