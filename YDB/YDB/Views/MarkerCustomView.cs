using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace YDB.Views
{
	public class MarkerCustomView : ContentView
	{
        public static List<RelativeLayout> rllist = new List<RelativeLayout>();

        Button MarkerButton { get; set; }
        Label checkMarkL;
        RelativeLayout rltest;

        public string HexColor { get; set; }
        public bool Marked { get; set; }

        public MarkerCustomView (string color, Color markColor, Action SetMarkerInToModel)
		{
            HexColor = color;

            MarkerButton = new Button()
            {
                BorderColor = Color.Gray,
                BorderWidth = 0.5,
                AnchorX = 0.5,
                AnchorY = 0.5,
                WidthRequest = 35,
                HeightRequest = 35,
                CornerRadius = 100,
                BackgroundColor = Color.FromHex(HexColor)
            };
            MarkerButton.Pressed += (s, e) =>
            {
                foreach (var item in rllist)
                {
                    if (item.Children[1].IsVisible == true)
                    {
                        item.Children[1].IsVisible = false;
                        Marked = false;
                    }
                }

                (MarkerButton.Parent as RelativeLayout).Children[1].IsVisible = true;
                Marked = true;

                SetMarkerInToModel();
            };

            checkMarkL = new Label()
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

            #region RelativeLayoutSettings

            rltest = new RelativeLayout() { HeightRequest = 35, WidthRequest = 35 };

            rltest.Children.Add(MarkerButton, Constraint.RelativeToParent((parent) =>
            {
                return parent.Width * 0;
            }), Constraint.RelativeToParent((parent) =>
            {
                return parent.Height * 0;
            }));

            rltest.Children.Add(checkMarkL, Constraint.RelativeToParent((parent) =>
            {
                return parent.Width * 0;
            }), Constraint.RelativeToParent((parent) =>
            {
                return parent.Height * 0;
            }));

            #endregion

            Content = rltest;
            rllist.Add(rltest);
        }
	}
}