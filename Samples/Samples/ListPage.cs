using System;
using System.Linq;
using SQLite;
using Xamarin.Forms;


namespace Samples {

    public class ListPage : ContentPage {
        private readonly ListView listView;


        public ListPage() {
            this.ToolbarItems.Add(new ToolbarItem {
                Text = "Clear",
                Command = new Command(() => App.Data.DeleteAll<RegionEvent>())
            });
            var tpl = new DataTemplate(typeof(TextCell));
            tpl.SetBinding(TextCell.TextProperty, "Description");
            tpl.SetBinding(TextCell.DetailProperty, "DateText");

            this.listView = new ListView {
                ItemTemplate = tpl,
                IsPullToRefreshEnabled = true,
                RefreshCommand = new Command(this.LoadData)
            };
        }


        protected override void OnAppearing() {
            base.OnAppearing();
            this.LoadData();
        }


        private void LoadData() {
            this.listView.IsRefreshing = true;
            this.listView.ItemsSource = App.Data
                .Table<RegionEvent>()
                .OrderBy(x => x.DateCreated)
                .Select(x => new RegionEventViewModel(x));
            this.listView.IsRefreshing = false;
        }
    }
}
