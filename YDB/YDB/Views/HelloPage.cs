using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace YDB.Views
{
    public class HelloPage : ContentPage
    {
        public HelloPage()
        {
            Title = "Меню";

            Content = new StackLayout
            {
                Children = {
                    new Label
                    {
                        FontFamily = App.fontNameMedium,
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        VerticalTextAlignment = TextAlignment.Center,
                        HorizontalTextAlignment = TextAlignment.Center,
                        FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                        TextColor = Color.Gray,
                        Text = "Привет!"
                    }
                }
            };
        }
    }
}