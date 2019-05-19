using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YDB.Models;
using Xamarin.Forms;

namespace YDB.Views
{
    //объект на странице "Добавить"
    //включает в себя Название поля + Сепаратор + Значение в Entry
	public class TableItemOnAdd : ContentView
	{
        public BoxView box;
        public Entry value;
        public Label key;
        public StackLayout main;
        public int Index { get; set; }

		public TableItemOnAdd(KeysAndTypes model, int index)
		{
            BindingContext = model;

            //Значение index - говорит о том, создал это поле пользователь или
            //данное поле загружено из базы
            Index = index;

            #region ViewSettings
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
            #endregion

            //если поле из базы, а не добавленое только что пользователем
            if (index != -1)
            {
                //пробуем получить данные об этом поле из базы
                if (model.Values.Count > 0)
                {
                    if (model.Values[index].Value != null)
                    {
                        value.Text = model.Values[index].Value;
                    }
                }
            }

            main = new StackLayout()
            {
                Children = { key, box, value }
            };

            Content = main;
		}
	}
}