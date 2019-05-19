using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace YDB.Views
{
    //объект одного маркера
	public class MarkerCustomView : ContentView
	{
        Button MarkerButton { get; set; }
        Label checkMarkL;
        public RelativeLayout rltest;

        public string HexColor { get; set; }
        public bool Marked { get; set; }

        //при создании передается: какой-необходим цвет маркера, потом цвет галочки и делегат,
        //который будет добавлять отмеченный маркер в модель
        public MarkerCustomView (string color, Color markColor, Action SetMarkerInToModel)
		{
            HexColor = color;

            #region Кнопка и её событие
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
                //отключаем все галочки
                foreach (MarkerCustomView item in ((s as Button).Parent.Parent.Parent as StackLayout).Children)
                {
                    if (item.rltest.Children[1].IsVisible == true)
                    {
                        item.rltest.Children[1].IsVisible = false;
                        item.Marked = false;
                    }
                }

                //ставим галочку на нажатом маркере
                (MarkerButton.Parent as RelativeLayout).Children[1].IsVisible = true;
                //устанавливает триггер нажатия
                ((s as Button).Parent.Parent as MarkerCustomView).Marked = true;

                //устанавливает маркер в модель
                SetMarkerInToModel();
            };
            #endregion

            //галочка
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
        }
	}
}