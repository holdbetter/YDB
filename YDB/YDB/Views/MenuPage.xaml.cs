using YDB.Models;
using YDB.ViewModels;
using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace YDB.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        List<HomeMenuItem> menuItems;
        ListView masterView;
        public Label hello, youNotLogin;
        public Label helloName, emptyList;
        StackLayout nonLoginView1, nonLoginView2;
        StackLayout emptyDBView1, emptyDBView2, DBListView;
        public Button btnGo;
        MenuPageViewModel menuPageViewModel;
        ImageButton imageButton;

        string username = "holdbetter";

        public StackLayout field1, field2;
        public ScrollView scr;

        public MenuPage()
        {
            BindingContext = menuPageViewModel = new MenuPageViewModel();

            Title = "Меню";

            #region field1
            field1 = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };

            #region nonLoginView1
            nonLoginView1 = new StackLayout()
            {
                Padding = new Thickness(25, 0, 25, 25),
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = Device.RuntimePlatform == Device.Android ? 150 : 100,
                BackgroundColor = Color.FromHex("#d83434")
            };

            hello = new Label()
            {
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.EndAndExpand,
                Text = "Привет!",
                FontFamily = App.fontNameBold,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                TextColor = Color.White
            };

            btnGo = new Button()
            {
                CornerRadius = 10,
                HorizontalOptions = LayoutOptions.EndAndExpand,
                VerticalOptions = LayoutOptions.EndAndExpand,
                Text = "Войти",
                TextColor = Color.White,
                BackgroundColor = Color.FromHex("#c91c1c"),
                FontFamily = App.fontNameMedium,
                HeightRequest = 40,
            };
            btnGo.SetBinding(Button.CommandProperty, "EnterInAppBtn");
            #endregion

            #region nonLoginView2
            nonLoginView2 = new StackLayout()
            {
                Padding = new Thickness(25),
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.StartAndExpand
            };

            youNotLogin = new Label()
            {
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Text = "Здесь\nты\nнайдешь\nсвои\nбазы\nданных,\nно сначала\nнужно\nвойти",
                FontFamily = App.fontNameRegular,
                FontSize = Device.RuntimePlatform == Device.UWP ? Device.GetNamedSize(NamedSize.Large, typeof(Label)) :
                    Device.GetNamedSize(NamedSize.Large, typeof(Label)) * 1.8,
                TextColor = Color.Gray
            };
            #endregion

            nonLoginView1.Children.Add(hello);
            nonLoginView1.Children.Add(btnGo);

            nonLoginView2.Children.Add(youNotLogin);

            field1.Children.Add(nonLoginView1);
            field1.Children.Add(nonLoginView2);
            #endregion

            #region field2
            field2 = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            emptyDBView1 = new StackLayout()
            {
                Padding = new Thickness(25, 0, 25, 25),
                HeightRequest = Device.RuntimePlatform == Device.Android ? 150 : 100,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.FromHex("#d83434")
            };

            emptyDBView2 = new StackLayout()
            {
                Padding = new Thickness(25),
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.Fill
            };

            helloName = new Label()
            {
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.EndAndExpand,
                Text = $"Привет!",
                FontFamily = App.fontNameBold,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                TextColor = Color.White
            };

            emptyList = new Label()
            {
                Text = "Создать",
                HorizontalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontFamily = App.fontNameRegular,
                FontSize = Device.RuntimePlatform == Device.UWP ? Device.GetNamedSize(NamedSize.Large, typeof(Label)) :
                    Device.GetNamedSize(NamedSize.Large, typeof(Label)) * 2,
                TextColor = Color.Gray
            };

            imageButton = new ImageButton()
            {
                Margin = new Thickness(0, 15, 0, 0),
                Source = Device.RuntimePlatform == Device.UWP ? "btnAddImgUWP.png" : "btnAddImg.png",
                BackgroundColor = Color.Transparent,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.Center,
                WidthRequest = 60,
                HeightRequest = 60,
            };
            imageButton.Pressed += ImageButton_Clicked;
            imageButton.Released += ImageButton_Released;
            imageButton.SetBinding(ImageButton.CommandProperty, "BaseCreateButton");


            emptyDBView1.Children.Add(helloName);
            emptyDBView2.Children.Add(emptyList);
            emptyDBView2.Children.Add(imageButton);

            field2.Children.Add(emptyDBView1);
            field2.Children.Add(emptyDBView2);

            #endregion

            scr = new ScrollView
            {
                Content = field1
            };

            Content = scr;
        }

        private void ImageButton_Released(object sender, EventArgs e)
        {
            imageButton.Scale = 1;
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            imageButton.Scale = 0.9;
        }
    }
}