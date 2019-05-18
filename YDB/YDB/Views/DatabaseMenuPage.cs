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
        public Frame view, edit, add;
        Frame bg1, bg2, bg3;
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

            editIm = new Image() { Source = "edit.png", WidthRequest = 70, HeightRequest = 70 };
            editText = new Label()
            {
                Margin = new Thickness(0, 0, 0, 0),
                HorizontalTextAlignment = TextAlignment.Center,
                Text = "Редактировать",
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
                //Content = new StackLayout()
                //{
                //    Children = { bg2 }
                //}
            };
            edit.GestureRecognizers.Add(editTapped);

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
                //Content = new StackLayout()
                //{
                //    Children = { bg3 }
                //}
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