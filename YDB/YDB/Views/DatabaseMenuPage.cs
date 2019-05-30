using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using YDB.Models;
using YDB.Services;

namespace YDB.Views
{
    //Page - отображающий Menu для созданной базы
    public class DatabaseMenuPage : ContentPage
    {
        public Frame view, edit, add;
        Frame bg1, bg2, bg3;
        Label viewText, editText, addtext;

        public Image viewIm, editIm, addIm;

        //будет перезаписываться при сохранении в других местах
        //и предоставлять свежие данные для все других страниц
        public static DbMenuListModel model;

        public DatabaseMenuPage(DbMenuListModel m)
        {
            model = m;
            BindingContext = model;
            this.SetBinding(TitleProperty, "Name");

            BackgroundColor = Color.White;

            #region Просмотр

            TapGestureRecognizer viewTapped = new TapGestureRecognizer();
            viewTapped.Tapped += async (obj, e) => {
                (obj as Frame).BackgroundColor = Color.FromHex("#c9c9c9");
                await Navigation.PushAsync(new DatabaseViewPage(model));
                (obj as Frame).BackgroundColor = Color.FromHex("#d83434");
            };

            viewIm = new Image() { Source = "view.png", WidthRequest = 70, HeightRequest = 70 };
            viewText = new Label()
            {
                Margin = new Thickness(0, 0, 0, 0),
                Text = "Просмотр",
                HorizontalTextAlignment = TextAlignment.Center,
                FontFamily = App.fontNameRegular,
                FontSize = Device.RuntimePlatform == Device.UWP ? 12 :
                           Device.GetNamedSize(NamedSize.Small, typeof(Label))
            };

            bg1 = new Frame()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = this.BackgroundColor,
                CornerRadius = 95,
                Content = new StackLayout
                {
                    Children = { viewIm, viewText }
                }
            };

            view = new Frame()
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Margin = Device.RuntimePlatform == Device.UWP ? new Thickness(50, 0, 0, 0) : 
                                                                new Thickness(0, 20, 0, 0),
                HeightRequest = 145,
                WidthRequest = 145,
                BackgroundColor = Color.FromHex("#d83434"),
                CornerRadius = 100,
                Padding = new Thickness(5),
                //Content = new StackLayout()
                //{
                //    Children = { bg1 }
                //}
                Content = bg1
            };
            view.GestureRecognizers.Add(viewTapped);

            #endregion

            #region Редактировать

            TapGestureRecognizer editTapped = new TapGestureRecognizer();
            editTapped.Tapped += async (obj, e) => {
                (obj as Frame).BackgroundColor = Color.FromHex("#c9c9c9");
                await Navigation.PushAsync(new DatabaseInfoEditPage(model));
                (obj as Frame).BackgroundColor = Color.FromHex("#d83434");
            };

            TapGestureRecognizer deleteTapped = new TapGestureRecognizer();
            deleteTapped.Tapped += async (obj, e) => { 
                (obj as Frame).BackgroundColor = Color.FromHex("#c9c9c9");

                bool response = await DisplayAlert("Удаление таблицы",
                    $"Вы уверены, что хотите удалить таблицу {model.Name}?\n" +
                    $"Это необратимое действие!", "Да", "Отмена");

                if (response)
                {
                    NavigationPage np = new NavigationPage(new CreateBasePage())
                    {
                        BarBackgroundColor = Color.FromHex("#d83434"),
                        BarTextColor = Color.White
                    };

                    var path = DependencyService.Get<IPathDatabase>().GetDataBasePath("ok3.db");

                    using (ApplicationContext db = new ApplicationContext(path))
                    {
                        var database = (from databases in db.DatabasesList.Include(data => data.UsersDatabases)
                                        .Include(data => data.DatabaseData).ThenInclude(th => th.Data).ThenInclude(th => th.Values)
                                        where databases.Id == model.Id
                                        select databases).FirstOrDefault();

                        var account = (from accounts in db.Accounts.Include(data => data.UsersDatabases)
                                       where accounts.Email == App.Gmail
                                       select accounts).FirstOrDefault();

                        foreach (var userBase in account.UsersDatabases)
                        {
                            if (model.Id == userBase.DbMenuListModelId)
                            {
                                account.UsersDatabases.Remove(userBase);
                                break;
                            }
                        }

                        var menupage = ((App.Current.MainPage as MainPage).Master as MenuPage).menuPageViewModel;
                        foreach (var item in menupage.DbList)
                        {
                            if (item.Id == database.Id)
                            {
                                menupage.DbList.Remove(item);
                                break;
                            }
                        }

                        await db.SaveChangesAsync();

                        (App.Current.MainPage as MainPage).Detail = np;
                    }
                }

                (obj as Frame).BackgroundColor = Color.FromHex("#d83434");
            }; //удаления таблицы не у владельца

            editIm = new Image() { Source = "edit.png", WidthRequest = 70, HeightRequest = 70 };
            editText = new Label()
            {
                Margin = new Thickness(0, 0, 0, 0),
                HorizontalTextAlignment = TextAlignment.Center,
                Text = App.Gmail == model.Carrier ? "Редактировать" : "Удалить",
                FontFamily = App.fontNameRegular,
                FontSize = Device.RuntimePlatform == Device.UWP ? 12 :
                           Device.GetNamedSize(NamedSize.Small, typeof(Label))
            };

            bg2 = new Frame()
            {
                BackgroundColor = this.BackgroundColor,
                VerticalOptions = LayoutOptions.FillAndExpand,
                CornerRadius = 95,
                Content = new StackLayout
                {
                    Children = { editIm, editText }
                }
            };

            edit = new Frame()
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Margin = Device.RuntimePlatform == Device.UWP ? new Thickness(0, 0, 0, 0) :
                                                                new Thickness(0, 20, 0, 0),
                HeightRequest = 145,
                WidthRequest = 145,
                BackgroundColor = Color.FromHex("#d83434"),
                CornerRadius = 100,
                Padding = new Thickness(5),
                Content = bg2
            };

            if (model.Carrier == App.Gmail)
            {
                edit.GestureRecognizers.Add(editTapped);
            }
            else
            {
                edit.GestureRecognizers.Add(deleteTapped);
            }
            #endregion

            #region Добавить

            TapGestureRecognizer addTapped = new TapGestureRecognizer();
            addTapped.Tapped += async (obj, e) => {
                (obj as Frame).BackgroundColor = Color.FromHex("#c9c9c9");
                await Navigation.PushAsync(new DatabaseAddItemPage(model));
                (obj as Frame).BackgroundColor = Color.FromHex("#d83434");
            };
            
            addIm = new Image() { Source = "add.png", WidthRequest = 60, HeightRequest = 60 };
            addtext = new Label()
            {
                Margin = new Thickness(0, 0, 0, 0),
                HorizontalTextAlignment = TextAlignment.Center,
                Text = "Добавить\nУдалить",
                FontFamily = App.fontNameRegular,
                FontSize = Device.RuntimePlatform == Device.UWP ? 12 :
                           Device.GetNamedSize(NamedSize.Small, typeof(Label))
            };

            bg3 = new Frame()
            {
                BackgroundColor = this.BackgroundColor,
                VerticalOptions = LayoutOptions.FillAndExpand,
                CornerRadius = 95,
                Content = new StackLayout()
                {
                    Children = { addIm, addtext }
                }
            };

            add = new Frame()
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Margin = Device.RuntimePlatform == Device.UWP ? new Thickness(50, 0, 0, 0) : 
                                                                new Thickness(0, 20, 0, 0),
                HeightRequest = 145,
                WidthRequest = 145,
                BackgroundColor = Color.FromHex("#d83434"),
                CornerRadius = 100,
                Padding = new Thickness(5),
                Content = bg3
            };
            add.GestureRecognizers.Add(addTapped);

            #endregion
            
            StackLayout main = new StackLayout()
            {
                Orientation = Device.RuntimePlatform == Device.UWP ? StackOrientation.Horizontal :
                                                                     StackOrientation.Vertical,
                Padding = new Thickness(20, 20),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            if (model.Carrier == App.Gmail)
            {
                main.Children.Add(edit);
                main.Children.Add(view);
                main.Children.Add(add);
            }
            else
            {
                main.Children.Add(view);
                main.Children.Add(add);
                main.Children.Add(edit);
            }

            ScrollView scr = new ScrollView()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Orientation = Device.RuntimePlatform == Device.UWP ? ScrollOrientation.Horizontal :
                                                                     ScrollOrientation.Vertical,
                Content = main
            };

            Content = scr;
        }

        protected override void OnAppearing()
        {
            this.BindingContext = model;

            base.OnAppearing();
        }
    }
}