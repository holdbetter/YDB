using YDB.Models;
using YDB.ViewModels;
using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;
using YDB.Services;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace YDB.Views
{
    public partial class MenuPage : ContentPage
    {
        MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        public MenuPageViewModel menuPageViewModel;

        public Label hello, youNotLogin;
        public Label helloName, emptyList, yourId;
        public Button btnGo, btnOut;
        public Span spYour, spId, spanGmail, hi;

        ImageButton imageButton;

        Frame createView;
        StackLayout nonLoginView1, nonLoginView2;
        public StackLayout emptyDBView1, emptyDBView2, DbStackListView, databaseListStack;
        public StackLayout field1, field2, field3;
        public ScrollView scr1;

        public MenuPage()
        {
            BindingContext = menuPageViewModel = new MenuPageViewModel();

            Title = "Меню";

            //незалогиненный View
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

            //залогиненный View
            #region field2
            field2 = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            emptyDBView1 = new StackLayout()
            {
                Padding = Device.RuntimePlatform == Device.UWP ? 
                new Thickness(25, 10, 25, 25) :
                new Thickness(25, 25, 25, 10),
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

            FormattedString fsYourEmail = new FormattedString();
            hi = new Span() { Text = "Привет!\n" };
            spanGmail = new Span() { Text = "" };

            spYour = new Span()
            {
                FontFamily = App.fontNameRegular, Text = "\nТвой id: ",
                FontSize = Device.RuntimePlatform == Device.UWP ?
                    Device.GetNamedSize(NamedSize.Micro, typeof(Label)) :
                    Device.GetNamedSize(NamedSize.Small, typeof(Label)),
            };
            spId = new Span()
            {
                FontFamily = App.fontNameRegular, Text = "" ,
                FontSize = Device.RuntimePlatform == Device.UWP ?
                    Device.GetNamedSize(NamedSize.Micro, typeof(Label)) :
                    Device.GetNamedSize(NamedSize.Small, typeof(Label)),
            };

            fsYourEmail.Spans.Add(hi);
            fsYourEmail.Spans.Add(spanGmail);
            fsYourEmail.Spans.Add(spYour);
            fsYourEmail.Spans.Add(spId);

            helloName = new Label()
            {
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.EndAndExpand,
                FormattedText = fsYourEmail,
                FontFamily = App.fontNameBold,
                FontSize = Device.RuntimePlatform == Device.UWP ?
                    Device.GetNamedSize(NamedSize.Small, typeof(Label)) :
                    Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                TextColor = Color.White
            };

            btnOut = new Button()
            {
                CornerRadius = 10,
                HorizontalOptions = LayoutOptions.EndAndExpand,
                VerticalOptions = LayoutOptions.EndAndExpand,
                Margin = new Thickness(0, 0, 0, 0),
                Text = "Выйти",
                TextColor = Color.White,
                BackgroundColor = Color.FromHex("#c91c1c"),
                FontFamily = App.fontNameMedium,
                HeightRequest = 40,
            };
            btnOut.Pressed += BtnOut_Pressed;

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
                Source = Device.RuntimePlatform == Device.UWP ? "btnAddImg.png" : "btnAddImg.png",
                BackgroundColor = Color.Transparent,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.Center,
                WidthRequest = 60,
                HeightRequest = 60
            };
            imageButton.Pressed += ImageButton_Clicked;
            imageButton.Released += ImageButton_Released;
            imageButton.SetBinding(ImageButton.CommandProperty, "BaseCreateButton");

            emptyDBView1.Children.Add(btnOut);
            emptyDBView1.Children.Add(helloName);
            emptyDBView2.Children.Add(emptyList);
            emptyDBView2.Children.Add(imageButton);

            field2.Children.Add(emptyDBView1);
            field2.Children.Add(emptyDBView2);
            #endregion

            //ListView баз данных
            #region field3

            List<string> list = new List<string>();

            TapGestureRecognizer createTapped = new TapGestureRecognizer();
            createTapped.Tapped += CreateView_ItemTapped;

            createView = new CustomFrame()
            {
                HasShadow = false,
                Padding = new Thickness(0),
                Margin = new Thickness(0, -5, 0, 0),
                CornerRadius = 0,
                HeightRequest = 55,
                VerticalOptions = LayoutOptions.Start,
                Content = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    Padding = new Thickness(25, 0),
                    Children =
                    {
                        new Image()
                        {
                            WidthRequest = 30,
                            HeightRequest = 30,
                            Source = "btnAddImg.png"
                        },
                        new Label()
                        {
                            Margin = new Thickness(15, 0, 0, 0),
                            FontFamily = App.fontNameRegular,
                            VerticalTextAlignment = TextAlignment.Center,
                            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                            TextColor = Color.Black,
                            Text = "Создать"
                        }
                    }
                }
            };
            createView.PropertyChanged += CreateView_PropertyChanged;
            createView.GestureRecognizers.Add(createTapped);


            databaseListStack = new StackLayout() { VerticalOptions = LayoutOptions.FillAndExpand };
            foreach (var item in menuPageViewModel.DbList)
            {
                ListDbView listDb = new ListDbView(item);
                databaseListStack.Children.Add(listDb);
            }

            field3 = new StackLayout()
            {
                BackgroundColor = Color.White,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children =
                {
                    createView,
                    new BoxView()
                    {
                        Margin = new Thickness(0, -5, 0, 0),
                        HeightRequest = 0.5,
                        HorizontalOptions = LayoutOptions.Fill,
                        BackgroundColor = Color.Gray
                    },
                    databaseListStack
                }
            };

            #endregion

            scr1 = new ScrollView
            {
                Content = field1
            };

            Content = scr1;
        }

        private void CreateView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Frame frame = sender as Frame;

            if (e.PropertyName == "BackgroundColor")
            {
                if (frame.BackgroundColor != Color.White)
                {
                    Timer timer = new Timer((f) =>
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            frame.BackgroundColor = Color.White;
                        });
                    },
                    null, 50, Timeout.Infinite);
                }
            }
        }

        private async void CreateView_ItemTapped(object sender, EventArgs e)
        {
            Frame frame = sender as Frame;
            frame.BackgroundColor = Color.LightGray;

            NavigationPage page = (App.Current.MainPage as MainPage).Detail as NavigationPage;

            if (!(page?.CurrentPage is CreateBasePage))
            {
                (App.Current.MainPage as MainPage).Detail = new NavigationPage(new CreateBasePage())
                {
                    BarBackgroundColor = Color.FromHex("#d83434"),
                    BarTextColor = Color.White
                };

                if (Device.RuntimePlatform == Device.Android)
                    await Task.Delay(100);
            }

            (App.Current.MainPage as MainPage).IsPresented = false;
        }
 
        private async void BtnOut_Pressed(object sender, EventArgs e)
        {
            menuPageViewModel.DbList.Clear();
            databaseListStack.Children.Clear();

            Application.Current.Properties.Remove("Email");
            Application.Current.Properties.Remove("Expires");
            Application.Current.Properties.Remove("Id");

            await App.Current.SavePropertiesAsync();

            (App.Current.MainPage as MainPage).Detail = new HelloPage();

            scr1.Content = field1;
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
                        ((sender as Label).Parent.Parent as ViewCell).IsEnabled = true;
                    }
                    else if ((sender as Label).ClassId == "false")
                    {
                        ((sender as Label).Parent.Parent as ViewCell).IsEnabled = false;
                        (sender as Label).Text = "Загрузка...";
                    }
                }
            }
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