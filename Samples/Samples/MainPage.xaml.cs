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
	}
}