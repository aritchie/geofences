using System;


namespace Samples.ViewModels
{
    public class HomeViewModel : AbstractViewModel
    {
        //43.6411314, -79.3808415 88 Queen's Quay West
            // outside 3km fence - 43.6479868, -79.3713313

        //43.8477697, -79.0435461 Ajax GO Station
        public HomeViewModel(SettingsViewModel settings, HistoryViewModel history)
        {
            this.Settings = settings;
            this.History = history;
        }


        public SettingsViewModel Settings { get; }
        public HistoryViewModel History { get; }
    }
}
