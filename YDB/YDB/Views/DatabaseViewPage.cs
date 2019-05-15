using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using YDB.Models;

namespace YDB.Views
{
    public class DatabaseViewPage : ContentPage
    {
        //должна быть работа со списком Values - тупо переключение индексов

        List<Label> labels;

        //public static DbMenuListModel model;

        public DatabaseViewPage() { }


        public DatabaseViewPage(DbMenuListModel model)
        {
            BindingContext = model;

            labels = new List<Label>();

            foreach (var item in model.DatabaseData.Data)
            {
                Label key = new Label()
                {
                    FontFamily = App.fontNameMedium,
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                    TextColor = Color.Black
                };

                key.SetBinding(Label.TextProperty, "Key");

                labels.Add(key);

                foreach (var value in item.Values)
                {
                    Label field = new Label()
                    {
                        FontFamily = App.fontNameMedium,
                        FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                        TextColor = Color.Black
                    };

                    key.SetBinding(Label.TextProperty, "Value");

                    labels.Add(field);
                }
            }
        }
    }
}
