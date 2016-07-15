using System;


namespace Samples
{
    public interface IAppLifecycle
    {
        void OnAppResume();
        void OnAppSleep();
    }
}
