using System;
using System.Collections.Generic;
using System.Text;
using YDB.Models;
using Xamarin.Forms;
using System.Linq;
using YDB.Services;
using YDB.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace YDB.Views
{
    public class DatabaseInfoEditPage : ContentPage
    {
        StackLayout main, forSwitch, markersStack;
        Button editFieldPageBtn, deleteDatabaseBtn;
        Label infoL, safetyL, markersL, nonPublic;
        Entry name;
        Switch isPublic;
        ScrollView markerScroll;
        ToolbarItem toolbarItem;

        List<Entry> entriesOfInvitedId = new List<Entry>();

        public static DbMenuListModel model;

        public DatabaseInfoEditPage(DbMenuListModel mod)
        {
            model = mod;
            BindingContext = model;
            this.SetBinding(TitleProperty, "Name");

            main = new StackLayout();

            #region Toolbar
            toolbarItem = new ToolbarItem();
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

            deleteDatabaseBtn = new Button()
            {
                Margin = new Thickness(5, 5),
                BorderWidth = 1.5,
                BorderColor = Color.FromHex("#d83434"),
                BackgroundColor = Color.White,
                Text = "Удалить таблицу",
                TextColor = Color.FromHex("#d83434"),
                FontFamily = App.fontNameMedium,
                Command = new Command(DeleteDatabase),
                CommandParameter = this.BindingContext,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                CornerRadius = 5
            }; //Удалить таблицу

            if (App.Gmail == model.Carrier)
            {
                #region Add all features

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
                }; //Ввести название
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

                    var path = DependencyService.Get<IPathDatabase>().GetDataBasePath("ok3.db");
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
                    privateId.TextChanged += EnterId_TextChanged;

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

                    if (App.Gmail != item.DbAccountModelEmail)
                    {
                        entriesOfInvitedId.Add(privateId);
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
                    Command = new Command(async () =>
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

                main.Children.Add(deleteDatabaseBtn);

                #endregion
            }

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

        protected override void OnAppearing()
        {
            this.BindingContext = model;

            base.OnAppearing();
        }

        private void CreateField()
        {
            Entry enterId = new Entry()
            {
                AutomationId = "entry",
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Placeholder = "id пользователя",
                FontFamily = App.fontNameRegular,
                Keyboard = Keyboard.Numeric
            };
            enterId.TextChanged += EnterId_TextChanged;

            StackLayout sl = new StackLayout()
            {
                AutomationId = "generatedField",
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(15, 0, 10, 0),
                Orientation = StackOrientation.Horizontal,
                Children =
                {
                    enterId,
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

        private void EnterId_TextChanged(object sender, TextChangedEventArgs e)
        {
            Entry enterId = sender as Entry;

            Regex matching = new Regex(@"^\d*$");

            if (!matching.IsMatch(enterId.Text))
            {
                Regex deleteSymbols = new Regex(@"\D");
                enterId.Text = deleteSymbols.Replace(enterId.Text, "");
            }
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
            toolbarItem.IsEnabled = false;

            DbMenuListModel modelNew = obj as DbMenuListModel;

            if (name.Text != null && name.Text != "")
            {
                string text = name.Text.Trim();
                string firstLetter = text[0].ToString();
                string result = (text.Remove(0, 1)).Insert(0, firstLetter.ToUpper());

                if (result.Count() < 4)
                {
                    await DisplayAlert("Возникла проблема",
                                        "Название базы данных должно иметь больше 3 символов",
                                        "ОК");
                    toolbarItem.IsEnabled = true;
                    return;
                }
                else if (result.Count() > 12)
                {
                    await DisplayAlert("Возникла проблема",
                                       "Название базы данных не может иметь больше 12 символов",
                                       "ОК");
                    toolbarItem.IsEnabled = true;
                    return;
                }

                modelNew.Name = result;
            }
            else
            {
                await DisplayAlert("Возникла проблема",
                                        "Название базы данных должно иметь больше 3 символов",
                                        "ОК");
                toolbarItem.IsEnabled = true;
                return;
            }

            string path = DependencyService.Get<IPathDatabase>().GetDataBasePath("ok3.db");

            using (ApplicationContext db = new ApplicationContext(path))
            {
                var database = (from databases in db.DatabasesList.Include(data => data.UsersDatabases)
                                .Include(data => data.DatabaseData).ThenInclude(th => th.Data).ThenInclude(th => th.Values)
                                where databases.Id == modelNew.Id
                                select databases).FirstOrDefault();

                var menupage = ((App.Current.MainPage as MainPage).Master as MenuPage).menuPageViewModel;

                database.Name = modelNew.Name;
                database.Marker = modelNew.Marker;
                database.IsPrivate = modelNew.IsPrivate;

                db.SaveChanges();

                List<int> usersId = new List<int>();

                if (!database.IsPrivate)
                {
                    for (int i = database.UsersDatabases.Count - 1; i >= 0; i--)
                    {
                        if (database.UsersDatabases[i].DbAccountModelEmail != database.Carrier)
                        {
                            database.UsersDatabases.Remove(database.UsersDatabases[i]);
                        }
                    }
                }
                else
                {
                    //собираем новые id
                    foreach (Entry entry in entriesOfInvitedId)
                    {
                        if (entry.Text != null && entry.Text != "")
                        {
                            if (!usersId.Contains(Convert.ToInt32(entry.Text)))
                            {
                                usersId.Add(Convert.ToInt32(entry.Text));
                            }
                        }
                    }

                    foreach (var id in usersId)
                    {
                        if (modelNew.IsPrivate && usersId.Count > 0)
                        {
                            var a = (from account in db.Accounts.Include(us => us.UsersDatabases)
                                     where account.Number == id
                                     select account).FirstOrDefault();

                            bool isEmpty = true;

                            if (modelNew != null && a != null)
                            {
                                //если у пользователя уже есть данная, база, то не добавляем его еще раз
                                foreach (var user in database.UsersDatabases)
                                {
                                    if (user.DbAccountModelEmail == a.Email)
                                    {
                                        isEmpty = false;
                                        break;
                                    }
                                }

                                if (isEmpty)
                                {
                                    a.UsersDatabases.Add(new UsersDatabases()
                                    {
                                        DbMenuListModel = modelNew,
                                        DbAccountModel = a
                                    });

                                    db.SaveChanges();
                                }
                            }
                        }
                    }

                    for (int i = database.UsersDatabases.Count - 1; i >= 0; i--)
                    {
                        bool shouldBeDeleted = true;
                        foreach (var uId in usersId)
                        {
                            var objUser = (from accounts in db.Accounts.ToList()
                                           where accounts.Number == uId
                                           select accounts).FirstOrDefault();

                            if (objUser != null)
                            {
                                if (database.UsersDatabases[i].DbAccountModelEmail == objUser.Email)
                                {
                                    shouldBeDeleted = false;
                                }
                            }
                        }

                        if (shouldBeDeleted)
                        {
                            if (database.UsersDatabases[i].DbAccountModelEmail != database.Carrier)
                            {
                                database.UsersDatabases.Remove(database.UsersDatabases[i]);
                            }
                        }
                    }
                }

                for (int i = 0; i < menupage.DbList.Count; i++)
                {
                    if (menupage.DbList[i].Id == database.Id)
                    {
                        database.IsLoading = "true";

                        menupage.DbList.RemoveAt(i);

                        menupage.DbList.Insert(i, database);

                        break;
                    }
                }
                DatabaseMenuPage.model = database;

                toolbarItem.IsEnabled = true;

                await db.SaveChangesAsync();
                await Navigation.PopAsync();
            }
        }

        private async void DeleteDatabase(object obj)
        {
            DbMenuListModel model = obj as DbMenuListModel;

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

                    if (App.Gmail == database.Carrier)
                    {
                        db.DatabasesList.Remove(database);
                    }
                    

                    db.SaveChanges();

                    (App.Current.MainPage as MainPage).Detail = np;
                }
            }
        }
    }
}
