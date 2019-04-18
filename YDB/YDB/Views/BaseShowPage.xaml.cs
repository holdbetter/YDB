using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace YDB.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BaseShowPage : ContentPage
	{
		public BaseShowPage ()
		{
			InitializeComponent ();
		}

        private void SwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e)
        {
            (App.Current.MainPage as MainPage).IsPresented = true;
        }
    }
}