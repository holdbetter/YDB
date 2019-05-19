using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YDB.Views;
using YDB.Models;
using Xamarin.Forms;
using System.Linq;

namespace YDB.Views
{
    //Page отображающий конкретный объект
    public class DatabaseObjectViewPage : ContentPage
    {
        public DatabaseObjectViewPage(DbMenuListModel model, int id)
        {
            this.BindingContext = model;

            this.SetBinding(TitleProperty, "Name");

            StackLayout main = new StackLayout()
            {
                Padding = Device.RuntimePlatform == Device.UWP ? new Thickness(50, 30) : new Thickness(20, 20)
            };

            //нужен для загрузки полей объекта
            int trueId = -1;

            //загрузка полей
            foreach (KeysAndTypes item in model.DatabaseData.Data)
            {
                #region FormattedString для *Название_поля* : *Значение*
                FormattedString fs = new FormattedString();
                Span key = new Span() { BindingContext = item };
                key.SetBinding(Span.TextProperty, "Key");
            
                Span separator = new Span() { Text = ": " };

                fs.Spans.Add(key);
                fs.Spans.Add(separator);
                #endregion

                #region Ключ
                Label keyLab = new Label()
                {
                    FormattedText = fs,
                    FontFamily = App.fontNameRegular,
                    FontSize = Device.RuntimePlatform == Device.UWP ?
                               Device.GetNamedSize(NamedSize.Small, typeof(Label)) :
                               Device.GetNamedSize(NamedSize.Medium, typeof(Label))
                };

                //добавление ключа в стэк
                StackLayout field = new StackLayout()
                {
                    Margin = new Thickness(0, 5, 0, 0),
                    Orientation = StackOrientation.Horizontal,
                    Children = { keyLab }
                };
                #endregion

                #region Значение
                //вытягивает Id по которому можно будет обратиться к каждому Values ключа,
                //чтобы вытянуть корректный Value
                for (int i = 0; i < item.Values.Count; i++)
                {
                    if (item.Values[i].Id == id)
                    {
                        trueId = i;
                        break;
                    }
                }

                //если Id был найден
                if (trueId != -1)
                {
                    //если значения по ключу не пустые
                    if (item.Values.Count > 0)
                    {
                        //создает лейбл с Value ключа для объекта
                        Label value = new Label()
                        {
                            BindingContext = item.Values[trueId],
                            FontFamily = App.fontNameRegular,
                            FontSize = Device.RuntimePlatform == Device.UWP ?
                               Device.GetNamedSize(NamedSize.Small, typeof(Label)) :
                               Device.GetNamedSize(NamedSize.Medium, typeof(Label))
                        };
                        value.SetBinding(Label.TextProperty, "Value");
                        field.Children.Add(value);
                    }
                }
                main.Children.Add(field);
                #endregion

                ScrollView scr = new ScrollView()
                {
                    Content = main
                };

                Content = scr;
            }
        }
    }
}