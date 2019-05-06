using YDB.Models;
using YDB.ViewModels;
using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;

namespace YDB.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        public MenuPageViewModel menuPageViewModel;

        ListView DbListView, createView;
        public Label hello, youNotLogin;
        public Label helloName, emptyList;
        public Button btnGo;
        
        ImageButton imageButton;

        StackLayout nonLoginView1, nonLoginView2;
        public StackLayout emptyDBView1, emptyDBView2, DbStackListView;
        public StackLayout field1, field2, field3;
        public ScrollView scr1;

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
                HeightRequest = 60
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

            #region field3

            createView = new ListView()
            {
                ItemsSource = new List<EmptyModel>() { new EmptyModel() },
                Margin = new Thickness(0, -5, 0, 0),
                HasUnevenRows = false,
                SeparatorVisibility = SeparatorVisibility.None,
                RowHeight = 55,
                VerticalOptions = LayoutOptions.Start,
                ItemTemplate = new DataTemplate(() =>
                {
                    ViewCell viewCell = new ViewCell()
                    {
                        View = new StackLayout()
                        {
                            Orientation = StackOrientation.Horizontal,
                            Padding = new Thickness(25, 0),
                            HeightRequest = 55,
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
                    return viewCell;
                })
            };
            createView.ItemSelected += CreateView_ItemSelected;
            createView.ItemTapped += CreateView_ItemTapped;

            DbListView = new ListView()
            {
                Margin = new Thickness(0, -5, 0, 0),
                HasUnevenRows = false,
                SeparatorVisibility = SeparatorVisibility.None,
                RowHeight = 55,
                ItemTemplate = new DataTemplate(() =>
                {
                    Button button = new Button()
                    {
                        BorderColor = Color.Gray,
                        BorderWidth = 0.5,
                        AnchorX = 0.5,
                        AnchorY = 0.5,
                        WidthRequest = 30,
                        HeightRequest = 30,
                        CornerRadius = 100,
                        IsEnabled = false,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center
                    };
                    button.SetBinding(Button.BackgroundColorProperty, "MarkerColor");

                    Label label = new Label()
                    {
                        Margin = new Thickness(15, 0, 0, 0),
                        FontFamily = App.fontNameRegular,
                        VerticalTextAlignment = TextAlignment.Center,
                        FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                        TextColor = Color.Black,
                    };
                    label.SetBinding(Label.TextProperty, "Name");

                    return new ViewCell()
                    {
                        View = new StackLayout
                        {
                            VerticalOptions = LayoutOptions.Center,
                            Padding = new Thickness(25, 0),
                            Orientation = StackOrientation.Horizontal,
                            Children =
                            {
                                button,
                                label
                            }
                        }
                    };
                })
            };
            DbListView.SetBinding(ListView.ItemsSourceProperty, "DbList");
            DbListView.ItemSelected += DbListView_ItemSelected;

            field3 = new StackLayout()
            {
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
                    DbListView
                }
            };

            #endregion
            scr1 = new ScrollView
            {
                Content = field1
            };

            Content = scr1;
        }

        private async void CreateView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            NavigationPage page = (App.Current.MainPage as MainPage).Detail as NavigationPage;

            if (!(page.CurrentPage is CreateBasePage))
            {
                (App.Current.MainPage as MainPage).Detail = new NavigationPage(new CreateBasePage())
                {
                    BarBackgroundColor = Color.FromHex("#d83434"),
                };

                if (Device.RuntimePlatform == Device.Android)
                    await Task.Delay(100);
            }

            (App.Current.MainPage as MainPage).IsPresented = false;
        }

        private void CreateView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }

        private void DbListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
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