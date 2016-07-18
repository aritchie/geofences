using System;
using ReactiveUI;


namespace Samples.ViewModels
{
    public abstract class AbstractViewModel : ReactiveObject, IViewModel
    {
        public virtual void OnActivate()
        {
        }


        public virtual void OnDeactivate()
        {
        }


        public virtual bool OnBack()
        {
            return true;
        }


        public virtual void Init(object args)
        {
        }
    }
}
