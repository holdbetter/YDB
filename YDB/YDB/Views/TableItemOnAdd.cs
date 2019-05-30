using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YDB.Models;
using Xamarin.Forms;
using System.Text.RegularExpressions;

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
                Text = model.Type == "Номер телефона" ? "+" : null,
                Placeholder = "Значение"
            };

            if (model.Type == "Номер телефона")
            {
                value.Keyboard = Keyboard.Numeric;
            }
            else if (model.Type == "Число")
            {
                value.Keyboard = Keyboard.Numeric;
            }

            value.TextChanged += Value_TextChanged;
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

        private void Value_TextChanged(object sender, TextChangedEventArgs e)
        {
            KeysAndTypes model = BindingContext as KeysAndTypes;
            Entry entry = sender as Entry;
            int count = entry.Text.Count();

            if (model.Type == "Номер телефона")
            {
                Regex deleteSymbols = new Regex(@"^\d*$");

                if (!deleteSymbols.IsMatch(entry.Text) && entry.Text.Length > 0)
                {
                    string str = entry.Text;

                    if (entry.Text != "" && entry.Text[0] != '+')
                    {
                         str = entry.Text.TrimStart('+');
                    }

                    Regex t = new Regex(@"\D");
                    str = t.Replace(str, "");
                    entry.Text = "+" + str;
                }
                else if(entry.Text == "")
                {
                    int plusIndex = -1;
                    plusIndex = entry.Text.IndexOf('+');
                    if (plusIndex != -1)
                    {
                        entry.Text.Remove(plusIndex, 1);
                    }

                    entry.Text = entry.Text.Insert(0, "+");
                }
            }
            else if (model.Type == "Число")
            {
                Regex regex = new Regex(@"\D");
                entry.Text = regex.Replace(entry.Text, "");
            }
        }
    }
}