using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using YDB.Models;

namespace YDB.Views
{
    public class DatabaseMenuPage : ContentPage
    {
        public StackLayout view, edit, add;
        StackLayout bg1, bg2, bg3;
        Label viewText, editText, addtext;

        public Image viewIm, editIm, addIm;

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
                (obj as StackLayout).BackgroundColor = Color.FromHex("#c9c9c9");
                await Navigation.PushAsync(new DatabaseViewPage());
                (obj as StackLayout).BackgroundColor = Color.FromHex("#d83434");
            };

            viewIm = new Image() { Source = "view.png", WidthRequest = 80, HeightRequest = 80 };
            viewText = new Label()
            {
                Margin = new Thickness(0, 0, 0, 0),
                Text = "Просмотр",
                HorizontalTextAlignment = TextAlignment.Center,
                FontFamily = App.fontNameRegular,
                FontSize = Device.RuntimePlatform == Device.UWP ? 12 :
                           Device.GetNamedSize(NamedSize.Small, typeof(Label))
            };

            bg1 = new StackLayout()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = this.BackgroundColor,
                Children = { viewIm, viewText }
            };

            view = new StackLayout()
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Margin = Device.RuntimePlatform == Device.UWP ? new Thickness(50, 0, 0, 0) : 
                                                                new Thickness(0, 20, 0, 0),
                HeightRequest = 120,
                WidthRequest = 120,
                BackgroundColor = Color.FromHex("#d83434"),
                Padding = new Thickness(5),
                Children = { bg1 }
            };
            view.GestureRecognizers.Add(viewTapped);

            #endregion

            #region Редактировать

            TapGestureRecognizer editTapped = new TapGestureRecognizer();
            editTapped.Tapped += async (obj, e) => {
                (obj as StackLayout).BackgroundColor = Color.FromHex("#c9c9c9");
                await Navigation.PushAsync(new DatabaseInfoEditPage(model));
                (obj as StackLayout).BackgroundColor = Color.FromHex("#d83434");
            };

            editIm = new Image() { Source = "edit.png", WidthRequest = 80, HeightRequest = 80 };
            editText = new Label()
            {
                Margin = new Thickness(0, 0, 0, 0),
                HorizontalTextAlignment = TextAlignment.Center,
                Text = "Редактировать",
                FontFamily = App.fontNameRegular,
                FontSize = Device.RuntimePlatform == Device.UWP ? 12 :
                           Device.GetNamedSize(NamedSize.Small, typeof(Label))
            };

            bg2 = new StackLayout()
            {
                BackgroundColor = this.BackgroundColor,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children = { editIm, editText }
            };

            edit = new StackLayout()
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Margin = Device.RuntimePlatform == Device.UWP ? new Thickness(0, 0, 0, 0) :
                                                                new Thickness(0, 20, 0, 0),
                HeightRequest = 120,
                WidthRequest = 120,
                BackgroundColor = Color.FromHex("#d83434"),
                Padding = new Thickness(5),
                Children = { bg2 }
            };
            edit.GestureRecognizers.Add(editTapped);

            #endregion

            #region Добавить

            TapGestureRecognizer addTapped = new TapGestureRecognizer();
            addTapped.Tapped += async (obj, e) => {
                (obj as StackLayout).BackgroundColor = Color.FromHex("#c9c9c9");
                await Navigation.PushAsync(new DatabaseAddItemPage(model));
                (obj as StackLayout).BackgroundColor = Color.FromHex("#d83434");
            };
            
            addIm = new Image() { Source = "add.png", WidthRequest = 80, HeightRequest = 80 };
            addtext = new Label()
            {
                Margin = new Thickness(0, 0, 0, 0),
                HorizontalTextAlignment = TextAlignment.Center,
                Text = "Добавить",
                FontFamily = App.fontNameRegular,
                FontSize = Device.RuntimePlatform == Device.UWP ? 12 :
                           Device.GetNamedSize(NamedSize.Small, typeof(Label))
            };

            bg3 = new StackLayout()
            {
                BackgroundColor = this.BackgroundColor,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children = { addIm, addtext }
            };

            add = new StackLayout()
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Margin = Device.RuntimePlatform == Device.UWP ? new Thickness(50, 0, 0, 0) : 
                                                                new Thickness(0, 20, 0, 0),
                HeightRequest = 120,
                WidthRequest = 120,
                BackgroundColor = Color.FromHex("#d83434"),
                Padding = new Thickness(5),
                Children = { bg3 }
            };
            add.GestureRecognizers.Add(addTapped);

            #endregion

            StackLayout main = new StackLayout()
            {
                Orientation = Device.RuntimePlatform == Device.UWP ? StackOrientation.Horizontal :
                                                                     StackOrientation.Vertical,
                Padding = new Thickness(20, 20),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children = { edit, view, add }
            };

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