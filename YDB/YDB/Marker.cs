using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace YDB
{
	public class Marker : ContentView
	{
        public static List<RelativeLayout> rllist = new List<RelativeLayout>();
        Button btnttt;
        Label labelttt;
        RelativeLayout rltest;

        public Marker (Color buttonColor, Color markColor)
		{
            btnttt = new Button()
            {
                BorderColor = Color.Gray,
                BorderWidth = 0.5,
                AnchorX = 0.5,
                AnchorY = 0.5,
                WidthRequest = 35,
                HeightRequest = 35,
                CornerRadius = 100,
                BackgroundColor = buttonColor
            };

            labelttt = new Label()
            {
                HeightRequest = 35,
                WidthRequest = 35,
                Text = "", //checkmark symbol
                FontFamily = "Seguisym.ttf#Segoe UI Symbol",
                TextColor = markColor,
                FontSize = Device.RuntimePlatform == Device.UWP ? 12 : Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                BackgroundColor = Color.Transparent,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                IsVisible = false
            };

            rltest = new RelativeLayout() { HeightRequest = 35, WidthRequest = 35 };

            rltest.Children.Add(btnttt, Constraint.RelativeToParent((parent) =>
            {
                return parent.Width * 0;
            }), Constraint.RelativeToParent((parent) =>
            {
                return parent.Height * 0;
            }));

            rltest.Children.Add(labelttt, Constraint.RelativeToParent((parent) =>
            {
                return parent.Width * 0;
            }), Constraint.RelativeToParent((parent) =>
            {
                return parent.Height * 0;
            }));

            btnttt.Command = new Command(() =>
            {
                foreach (var item in rllist)
                {
                    if (item.Children[1].IsVisible == true)
                    {
                        item.Children[1].IsVisible = false;
                    }
                }

                (btnttt.Parent as RelativeLayout).Children[1].IsVisible = true;
            });

            Content = rltest;
            rllist.Add(rltest);
        }
	}
}