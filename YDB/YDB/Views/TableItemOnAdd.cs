using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YDB.Models;
using Xamarin.Forms;

namespace YDB.Views
{
	public class TableItemOnAdd : ContentView
	{
        public BoxView box;
        public Entry value;
        public Label key;
        public StackLayout main;

		public TableItemOnAdd(KeysAndTypes model)
		{
            BindingContext = model;

            key = new Label()
            {
                Margin = new Thickness(15, 10, 15, 0),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center,
                FontFamily = App.fontNameRegular,
                FontSize = Device.RuntimePlatform == Device.UWP ? Device.GetNamedSize(NamedSize.Micro, typeof(Label)) :
                                                                  Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                TextColor = Color.FromHex("#d83434")
            };
            key.SetBinding(Label.TextProperty, "Key");

            box = new BoxView()
            {
                Margin = new Thickness(0, 5, 0, 0),
                HeightRequest = 0.5,
                HorizontalOptions = LayoutOptions.Fill,
                BackgroundColor = Color.FromHex("#d83434")
            };

            value = new Entry()
            {
                Margin = new Thickness(15, 0, 15, 0),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                FontFamily = App.fontNameRegular,
                Placeholder = "Значение"
            };

            main = new StackLayout()
            {
                Children = { key, box, value }
            };

            Content = main;
		}
	}
}