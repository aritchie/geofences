using System;
using Acr;
using ReactiveUI;


namespace Samples.ViewModels
{
    public interface IViewModel : IReactiveObject, IViewModelLifecycle
    {
        void Init(object args = null);
    }
}
