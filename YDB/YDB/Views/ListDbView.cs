using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using YDB.Models;
using YDB.Services;

namespace YDB.Views
{
    public class ListDbView : ContentView
    {
        public ListDbView(DbMenuListModel model)
        {
            BindingContext = model;

            Button button = new Button()
            {
                ClassId = "",
                BorderColor = Color.Gray,
                BorderWidth = 0.5,
                AnchorX = 0.5,
                AnchorY = 0.5,
                WidthRequest = 30,
                HeightRequest = 30,
                CornerRadius = 100,
                IsEnabled = Device.RuntimePlatform == Device.UWP ? true : false,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            button.SetBinding(Button.BackgroundColorProperty, "HexColor");

            Label label = new Label()
            {
                Margin = new Thickness(15, 0, 0, 0),
                FontFamily = App.fontNameRegular,
                VerticalTextAlignment = TextAlignment.Center,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                TextColor = Color.Black
            };
            label.PropertyChanged += Label_PropertyChanged;
            label.SetBinding(Label.ClassIdProperty, "IsLoading");

            StackLayout main = new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                Padding = new Thickness(25, 0),
                Orientation = StackOrientation.Horizontal,
                Children =
                {
                    button,
                    label
                }
            };

            CustomFrame customFrame = new CustomFrame
            {
                HasShadow = false,
                BackgroundColor = Color.White,
                Padding = new Thickness(0),
                Margin = new Thickness(0, -5, 0, 0),
                CornerRadius = 0,
                HeightRequest = 55,
                VerticalOptions = LayoutOptions.Start,
                Content = main
            };
            customFrame.PropertyChanged += CustomFrame_PropertyChanged;

            TapGestureRecognizer listDbTap = new TapGestureRecognizer();
            listDbTap.Tapped += DbListView_ItemTappedAsync;
            customFrame.GestureRecognizers.Add(listDbTap);

            Content = customFrame;
        }

        private void CustomFrame_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Frame thisFrame = sender as Frame;

            if (e.PropertyName == "BackgroundColor")
            {
                if (thisFrame.BackgroundColor != Color.White)
                {
                    Timer timer = new Timer((f) =>
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            thisFrame.BackgroundColor = Color.White;
                        });
                    },
                    null, 50, Timeout.Infinite);
                }
            }
        }

        private async void DbListView_ItemTappedAsync(object sender, EventArgs e)
        {
            Frame frame = sender as Frame;
            frame.BackgroundColor = Color.LightGray;

            var item = (sender as Element).BindingContext as DbMenuListModel;

            if (item.IsLoading == "false")
            {
                return;
            }

            var path = DependencyService.Get<IPathDatabase>().GetDataBasePath("ok3.db");

            using (ApplicationContext db = new ApplicationContext(path))
            {
                var obj = (from database in db.DatabasesList
                           .Include(m => m.DatabaseData).ThenInclude(ub => ub.Data).ThenInclude(ub => ub.Values)
                           .Include(m => m.UsersDatabases)
                           .ToList()
                           where database.Id == item.Id
                           select database).FirstOrDefault();

                (App.Current.MainPage as MainPage).Detail = new NavigationPage(new DatabaseMenuPage(obj))
                {
                    BarBackgroundColor = Color.FromHex("#d83434"),
                    BarTextColor = Color.White
                };

                if (Device.RuntimePlatform == Device.Android)
                {
                    await Task.Delay(100);
                }

                (App.Current.MainPage as MainPage).IsPresented = false;
            }
        }

        private void Label_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is Label)
            {
                if (e.PropertyName == "ClassId")
                {
                    if ((sender as Label).ClassId == "true")
                    {
                        (sender as Label).SetBinding(Label.TextProperty, "Name");
                        ((sender as Label).Parent as StackLayout).IsEnabled = true;
                    }
                    else if ((sender as Label).ClassId == "false")
                    {
                        ((sender as Label).Parent as StackLayout).IsEnabled = false;
                        (sender as Label).Text = "Загрузка...";
                    }
                }
            }
        }
    }
}