using System;
using System.Threading.Tasks;
using Samples.ViewModels;
using Xamarin.Forms;


namespace Samples.Services
{
    public interface IViewModelManager
    {
        TViewModel Create<TViewModel>(object args = null) where TViewModel : class, IViewModel;
		Page CreatePage<TViewModel>(object args = null) where TViewModel : class, IViewModel;
		Task PushNav<TViewModel>(object args = null) where TViewModel : class, IViewModel;
        Task PopNav();
    }
}
