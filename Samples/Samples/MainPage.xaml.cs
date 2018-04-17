using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace Samples
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : TabbedPage
	{
		public MainPage()
		{
			this.InitializeComponent();
		}


	    protected override void OnAppearing()
	    {
	        base.OnAppearing();
	        ((MainViewModel) this.BindingContext).Start();
	    }
	}
}