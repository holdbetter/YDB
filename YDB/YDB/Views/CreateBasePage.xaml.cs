using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace YDB.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CreateBasePage : ContentPage
	{
        Label infoL, safetyL, markersL, nonPublic;
        Entry name;
        StackLayout main, forSwitch, markersStack;
        Switch isPublic;
        ScrollView markerScroll;
        List<Entry> entriesOfInvitedId = new List<Entry>();

        public CreateBasePage ()
		{
            MarkerCustomView.rllist.Clear();

            Title = "Создание базы данных";

            main = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };

            SwipeGestureRecognizer swipeGesture = new SwipeGestureRecognizer()
            {
                Direction = SwipeDirection.Right
            };
            swipeGesture.Swiped += (s, e) => (App.Current.MainPage as MainPage).IsPresented = true;
            main.GestureRecognizers.Add(swipeGesture);

            #region Toolbar
            ToolbarItem toolbarItem = new ToolbarItem();

            if (Device.RuntimePlatform == Device.UWP)
            {
                toolbarItem.Icon = "checkMark.png";
                toolbarItem.Text = "Сохранить";
            }
            else
            {
                toolbarItem.Icon = "checkMark.png";
            }

            //toolbarItem.SetBinding(ToolbarItem.CommandProperty, "");
            ToolbarItems.Add(toolbarItem);
            #endregion

            #region Views Settings

            infoL = new Label()
            {
                Margin = new Thickness(15, 10, 15, 0),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center,
                Text = "Информация",
                FontFamily = App.fontNameRegular,
                FontSize = Device.RuntimePlatform == Device.UWP ? Device.GetNamedSize(NamedSize.Micro, typeof(Label)) :
                                                                  Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                TextColor = Color.FromHex("#d83434")
            }; //Информация

            safetyL = new Label()
            {
                Margin = new Thickness(15, 10, 15, 0),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center,
                Text = "Безопасность",
                FontFamily = App.fontNameRegular,
                FontSize = Device.RuntimePlatform == Device.UWP ? Device.GetNamedSize(NamedSize.Micro, typeof(Label)) :
                                                                  Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                TextColor = Color.FromHex("#d83434")
            }; //Безопасность

            markersL = new Label()
            {
                Margin = new Thickness(15, 10, 15, 0),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center,
                Text = "Маркеры",
                FontFamily = App.fontNameRegular,
                FontSize = Device.RuntimePlatform == Device.UWP ? Device.GetNamedSize(NamedSize.Micro, typeof(Label)) :
                                                                  Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                TextColor = Color.FromHex("#d83434")
            }; //Маркеры

            name = new Entry()
            {
                Margin = new Thickness(15, 0, 15, 0),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                FontFamily = App.fontNameRegular,
                Placeholder = "Название базы данных"
            }; //Ввести ID

            isPublic = new Switch()
            {
                HorizontalOptions = LayoutOptions.EndAndExpand,
                VerticalOptions = LayoutOptions.Center,
            }; //Приватный свитч
            isPublic.Toggled += IsPublic_Toggled;

            nonPublic = new Label()
            {
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                Text = $"Приватная база данных?",
                FontFamily = App.fontNameRegular,
                FontSize = Device.RuntimePlatform == Device.UWP ? Device.GetNamedSize(NamedSize.Micro, typeof(Label)) :
                                                                  Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                TextColor = Color.Gray
            }; //Приватная база?

            forSwitch = new StackLayout()
            {
                Padding = new Thickness(15, 0),
                Margin = new Thickness(0, 5, 0, 0),
                Orientation = StackOrientation.Horizontal,
                Children =
                {
                    nonPublic,
                    isPublic
                }
            }; //Гор стэк для свитча

            markersStack = new StackLayout()
            {
                Padding = new Thickness(15, 5),
                VerticalOptions = LayoutOptions.StartAndExpand,
                Orientation = StackOrientation.Horizontal,
                Children =
                {
                    new MarkerCustomView(Color.Black, Color.FromHex("#edece8")),
                    new MarkerCustomView(Color.White, Color.FromHex("#353535")),
                    new MarkerCustomView(Color.FromHex("#ed4444"), Color.FromHex("#edece8")),
                    new MarkerCustomView(Color.Lavender, Color.FromHex("#353535")),
                    new MarkerCustomView(Color.FromHex("#9d70ff"), Color.FromHex("#edece8")),
                    new MarkerCustomView(Color.PaleGoldenrod, Color.FromHex("#353535")),
                    new MarkerCustomView(Color.Silver, Color.FromHex("#edece8")),
                    new MarkerCustomView(Color.FromHex("#fff372"), Color.FromHex("#353535")),
                    new MarkerCustomView(Color.FromHex("#59d8ff"), Color.FromHex("#edece8")),
                    new MarkerCustomView(Color.FromHex("#ffcccc"), Color.FromHex("#353535")),
                    new MarkerCustomView(Color.FromHex("#afa100"), Color.FromHex("#edece8")),
                    new MarkerCustomView(Color.FromHex("#a29bfe"), Color.FromHex("#353535")),
                    new MarkerCustomView(Color.FromHex("#05c46b"), Color.FromHex("#edece8")),
                    new MarkerCustomView(Color.FromHex("#fffa65"), Color.FromHex("#353535")),
                    new MarkerCustomView(Color.FromHex("#cd84f1"), Color.FromHex("#edece8")),
                } //markers
            }; //Стэк маркеров

            markerScroll = new ScrollView()
            {
                Orientation = ScrollOrientation.Horizontal,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
                FlowDirection = FlowDirection.LeftToRight,
                Content = markersStack
            }; //Упаковка скролла для маркеров

            #endregion

            #region Main StackLayout Children

            main.Children.Add(infoL);
            main.Children.Add(new BoxView()
            {
                Margin = new Thickness(0, 5, 0, 0),
                HeightRequest = 0.5,
                HorizontalOptions = LayoutOptions.Fill,
                BackgroundColor = Color.FromHex("#d83434")
            });
            main.Children.Add(name);
            main.Children.Add(safetyL);
            main.Children.Add(new BoxView()
            {
                Margin = new Thickness(0, 5, 0, 0),
                HeightRequest = 0.5,
                HorizontalOptions = LayoutOptions.Fill,
                BackgroundColor = Color.FromHex("#d83434")
            });
            main.Children.Add(forSwitch);
            main.Children.Add(markersL);
            main.Children.Add(new BoxView()
            {
                Margin = new Thickness(0, 5, 0, 0),
                HeightRequest = 0.5,
                HorizontalOptions = LayoutOptions.Fill,
                BackgroundColor = Color.FromHex("#d83434")
            });
            main.Children.Add(markerScroll);
            main.Children.Add(new BoxView()
            {
                Margin = new Thickness(0, 5, 0, 0),
                HeightRequest = 0.5,
                HorizontalOptions = LayoutOptions.Fill,
                BackgroundColor = Color.Gray
            });

            #endregion

            ScrollView scroll = new ScrollView()
            {
                Content = main
            };
            Content = scroll;
		}

        private void IsPublic_Toggled(object sender, ToggledEventArgs e)
        {
            //local function
            void CreateField()
            {
                StackLayout sl = new StackLayout()
                {
                    AutomationId = "generatedField",
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Padding = new Thickness(15, 0, 10, 0),
                    Orientation = StackOrientation.Horizontal,
                    Children =
                    {
                        new Entry()
                        {
                            AutomationId = "entry",
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            Placeholder = "id пользователя",
                            FontFamily = App.fontNameRegular,
                            Keyboard = Keyboard.Numeric
                        },
                        new Button()
                        {
                            Margin = new Thickness(10, 0, 0, 0),
                            HorizontalOptions = LayoutOptions.End,
                            BackgroundColor = Color.Transparent,
                            TextColor = Color.FromHex("#d83434"),
                            Text = "+",
                            WidthRequest = 50,
                            HeightRequest = 50,
                            FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Button)),
                            Command = new Command(CreateField)
                        }
                    }
                };
                main.Children.Insert(6, sl);
                entriesOfInvitedId.Add(sl.Children.First() as Entry);
            }

            if (e.Value)
            {
                CreateField();
            }
            else if(!e.Value)
            {
                for (int i = 0; i < main.Children.Count; i++)
                {
                    if (!string.IsNullOrEmpty((main.Children[6]).AutomationId))
                    {
                        main.Children.RemoveAt(6);
                    }
                }

                entriesOfInvitedId.Clear();
            }
        }
    }
}