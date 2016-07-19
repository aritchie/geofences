using System;


namespace Samples.ViewModels
{
    public class HomeViewModel : AbstractViewModel
    {
        public HomeViewModel(SettingsViewModel settings, HistoryViewModel history)
        {
            this.Settings = settings;
            this.History = history;
        }


        public SettingsViewModel Settings { get; }
        public HistoryViewModel History { get; }


        public override void OnActivate()
        {
            base.OnActivate();
            this.Settings.OnActivate();
            this.History.OnActivate();
        }


        public override void OnDeactivate()
        {
            base.OnDeactivate();
            this.Settings.OnDeactivate();
            this.History.OnDeactivate();
        }
    }
}
