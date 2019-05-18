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

            int trueId = -1;

            foreach (KeysAndTypes item in model.DatabaseData.Data)
            {
                FormattedString fs = new FormattedString();
                Span key = new Span() { BindingContext = item };
                key.SetBinding(Span.TextProperty, "Key");

                Span separator = new Span() { Text = ": " };

                fs.Spans.Add(key);
                fs.Spans.Add(separator);

                Label keyLab = new Label()
                {
                    FormattedText = fs,
                    FontFamily = App.fontNameRegular,
                    FontSize = Device.RuntimePlatform == Device.UWP ?
                               Device.GetNamedSize(NamedSize.Small, typeof(Label)) :                
                               Device.GetNamedSize(NamedSize.Medium, typeof(Label))                    
                };

                for (int i = 0; i < item.Values.Count; i++)
                {
                    if (item.Values[i].Id == id)
                    {
                        trueId = i;
                        break;
                    }
                }

                StackLayout field = new StackLayout()
                {
                    Margin = new Thickness(0, 5, 0, 0),
                    Orientation = StackOrientation.Horizontal,
                    Children = { keyLab }
                };

                if (trueId != -1)
                {
                    if (item.Values.Count > 0)
                    {
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

                ScrollView scr = new ScrollView()
                {
                    Content = main
                };

                Content = scr;
            }
        }
    }
}