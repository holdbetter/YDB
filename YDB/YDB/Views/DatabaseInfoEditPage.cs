using System;
using System.Collections.Generic;
using System.Text;
using YDB.Models;
using Xamarin.Forms;
using System.Linq;
using YDB.Services;
using YDB.ViewModels;

namespace YDB.Views
{
    public class DatabaseInfoEditPage : ContentPage
    {
        StackLayout main, forSwitch, markersStack;
        Button editFieldPageBtn;
        Label infoL, safetyL, markersL, nonPublic;
        Entry name;
        Switch isPublic;
        ScrollView markerScroll;

        List<Entry> entriesOfInvitedId = new List<Entry>();

        public DatabaseInfoEditPage(DbMenuListModel model)
        {
            BindingContext = model;
            this.SetBinding(TitleProperty, "Name");

            main = new StackLayout();

            #region Toolbar
            ToolbarItem toolbarItem = new ToolbarItem();
            toolbarItem.Command = new Command(UpdateDataAsync);
            toolbarItem.CommandParameter = this.BindingContext;

            if (Device.RuntimePlatform == Device.UWP)
            {
                toolbarItem.Icon = "checkMark.png";
                toolbarItem.Text = "Сохранить";
            }
            else
            {
                toolbarItem.Icon = "checkMark.png";
            }
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
            name = new Entry()
            {
                Margin = new Thickness(15, 0, 15, 0),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                FontFamily = App.fontNameRegular,
                Placeholder = "Название базы данных"
            }; //Ввести ID
            name.SetBinding(Entry.TextProperty, "Name");

            main.Children.Add(infoL);
            main.Children.Add(new BoxView()
            {
                Margin = new Thickness(0, 5, 0, 0),
                HeightRequest = 0.5,
                HorizontalOptions = LayoutOptions.Fill,
                BackgroundColor = Color.FromHex("#d83434")
            });
            main.Children.Add(name);

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

            isPublic = new Switch()
            {
                HorizontalOptions = LayoutOptions.EndAndExpand,
                VerticalOptions = LayoutOptions.Center,
            }; //Приватный свитч
            isPublic.Toggled += IsPublic_Toggled;
            isPublic.SetBinding(Switch.IsToggledProperty, "IsPrivate");

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
            main.Children.Add(safetyL);
            main.Children.Add(new BoxView()
            {
                Margin = new Thickness(0, 5, 0, 0),
                HeightRequest = 0.5,
                HorizontalOptions = LayoutOptions.Fill,
                BackgroundColor = Color.FromHex("#d83434")
            });
            main.Children.Add(forSwitch);

            foreach (var item in model.UsersDatabases)
            {
                string id;

                var path = DependencyService.Get<IPathDatabase>().GetDataBasePath("ok2.db");
                using (ApplicationContext db = new ApplicationContext(path))
                {
                    id = (from account in db.Accounts
                              where account.Email == item.DbAccountModelEmail
                              select account).FirstOrDefault().Number.ToString();
                }

                Entry privateId = new Entry()
                {
                    AutomationId = "entry",
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Placeholder = "id пользователя",
                    FontFamily = App.fontNameRegular,
                    Keyboard = Keyboard.Numeric,
                    Text = id
                };

                StackLayout sl = new StackLayout()
                {
                    AutomationId = "generatedField",
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Padding = new Thickness(15, 0, 10, 0),
                    Orientation = StackOrientation.Horizontal,
                    Children =
                    {
                        privateId,
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

                if (item.DbAccountModelEmail != model.Carrier)
                {
                    main.Children.Add(sl);
                }
            }

            markersStack = new StackLayout()
            {
                Padding = new Thickness(15, 5),
                VerticalOptions = LayoutOptions.StartAndExpand,
                Orientation = StackOrientation.Horizontal,
                Children =
                {
                    new MarkerCustomView("#000000", Color.FromHex("#edece8"), IsMarkedCheck),
                    new MarkerCustomView("#ffffff", Color.FromHex("#353535"), IsMarkedCheck),
                    new MarkerCustomView("#ed4444", Color.FromHex("#edece8"), IsMarkedCheck),
                    new MarkerCustomView("#e6e6fa", Color.FromHex("#353535"), IsMarkedCheck),
                    new MarkerCustomView("#9d70ff", Color.FromHex("#edece8"), IsMarkedCheck),
                    new MarkerCustomView("#eee8aa", Color.FromHex("#353535"), IsMarkedCheck),
                    new MarkerCustomView("#c0c0c0", Color.FromHex("#edece8"), IsMarkedCheck),
                    new MarkerCustomView("#fff372", Color.FromHex("#353535"), IsMarkedCheck),
                    new MarkerCustomView("#59d8ff", Color.FromHex("#edece8"), IsMarkedCheck),
                    new MarkerCustomView("#ffcccc", Color.FromHex("#353535"), IsMarkedCheck),
                    new MarkerCustomView("#afa100", Color.FromHex("#edece8"), IsMarkedCheck),
                    new MarkerCustomView("#a29bfe", Color.FromHex("#353535"), IsMarkedCheck),
                    new MarkerCustomView("#05c46b", Color.FromHex("#edece8"), IsMarkedCheck),
                    new MarkerCustomView("#fffa65", Color.FromHex("#353535"), IsMarkedCheck),
                    new MarkerCustomView("#cd84f1", Color.FromHex("#edece8"), IsMarkedCheck),
                } //markers
            }; //Стэк маркеров

            foreach (var m in markersStack.Children)
            {
                if (m is MarkerCustomView)
                {
                    MarkerCustomView marker = m as MarkerCustomView;

                    if (marker.HexColor == model.HexColor)
                    {
                        marker.rltest.Children[1].IsVisible = true;
                        marker.Marked = true;
                        IsMarkedCheck();
                    }
                }
            }

            markerScroll = new ScrollView()
            {
                Orientation = ScrollOrientation.Horizontal,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
                FlowDirection = FlowDirection.LeftToRight,
                Content = markersStack
            }; //Упаковка скролла для маркеров

            editFieldPageBtn = new Button()
            {
                Margin = new Thickness(5, 5),
                BorderWidth = 1.5,
                BorderColor = Color.FromHex("#d83434"),
                BackgroundColor = Color.White,
                Text = "Редактировать поля",
                TextColor = Color.FromHex("#d83434"),
                FontFamily = App.fontNameMedium,
                Command = new Command(async() =>
                {
                    await Navigation.PushAsync(new DatabaseEditFieldPage(model));
                }),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                CornerRadius = 5
            }; //Редактировать поля

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
            main.Children.Add(editFieldPageBtn);
            #endregion

            #region Свайп для страницы
            SwipeGestureRecognizer swipeGesture = new SwipeGestureRecognizer()
            {
                Direction = SwipeDirection.Right
            };
            swipeGesture.Swiped += (s, e) => (App.Current.MainPage as MainPage).IsPresented = true;
            main.GestureRecognizers.Add(swipeGesture);
            #endregion

            Content = main;
        }

        private void CreateField()
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
            //entriesOfInvitedId.Add(sl.Children.First() as Entry);
        }

        private void IsPublic_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                CreateField();
            }
            else if (!e.Value)
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

        private void IsMarkedCheck()
        {
            DbMenuListModel model = this.BindingContext as DbMenuListModel;

            foreach (MarkerCustomView item in markersStack.Children)
            {
                if (item.Marked)
                {
                    (this.BindingContext as DbMenuListModel).Marker = item;
                }
            }
        }

        private async void UpdateDataAsync(object obj)
        {
            DbMenuListModel model = obj as DbMenuListModel;

            string path = DependencyService.Get<IPathDatabase>().GetDataBasePath("ok2.db");

            using (ApplicationContext db = new ApplicationContext(path))
            {
                var database = (from databases in db.DatabasesList
                                where databases.Id == model.Id
                                select databases).FirstOrDefault();


                var menupage = ((App.Current.MainPage as MainPage).Master as MenuPage).menuPageViewModel;

                foreach (var item in menupage.DbList)
                {
                    if (item.Id == model.Id)
                    {
                        menupage.DbList.Remove(item);
                        break;
                    }
                }

                database.Name = model.Name;
                database.Marker = model.Marker;
                database.IsPrivate = model.IsPrivate;

                menupage.DbList.Add(database);
                DatabaseMenuPage.model = database;

                await db.SaveChangesAsync();
                await Navigation.PopAsync();
            }
        }
    }
}
