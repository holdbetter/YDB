using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using Xamarin.Forms;
using YDB.Models;
using YDB.Services;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace YDB.Views
{
    public class BaseConstructorPage : ContentPage
    {
        ScrollView scr;
        ToolbarItem add;
        int buttonId;

        public StackLayout main;

        //сигнализирует о том, какой Frame нужно удалить
        public int ButtonId
        {
            get
            {
                return buttonId;
            }
            set
            {
                buttonId = value;
                Callback();
            }
        }

        public BaseConstructorPage(DbMenuListModel dbMenuListModel)
        {
            FieldCustomView.score = 0;
            Title = "Создайте поля";

            #region Toolbar

            add = new ToolbarItem();
            add.Command = new Command(SaveInfo);
            add.CommandParameter = dbMenuListModel;

            if (Device.RuntimePlatform == Device.UWP)
            {
                add.Icon = "checkMark.png";
                add.Text = "Новое поле";
            }
            else
            {
                add.Icon = "checkMark.png";
            }

            ToolbarItems.Add(add);

            #endregion

            #region StackLayout Settings
            main = new StackLayout()
            {
                Padding = new Thickness(10, 5, 10, 0),
                Children =
                {
                    new FieldCustomView(TapGestureRecognizer_Tapped, DeleteBtnReleaseV2),
                    new Button()
                    {
                        Margin = new Thickness(5, 5),
                        BorderWidth = 1.5,
                        BorderColor = Color.FromHex("#d83434"),
                        BackgroundColor = Color.White,
                        Text = "Добавить",
                        TextColor = Color.FromHex("#d83434"),
                        FontFamily = App.fontNameMedium,
                        Command = new Command(() => {
                            main.Children.Insert(main.Children.Count == 1 ? 0 : main.Children.Count - 1, 
                            new FieldCustomView(TapGestureRecognizer_Tapped, DeleteBtnReleaseV2));
                            FieldCustomView.score++;
                        }),
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        CornerRadius = 5
                    }
                }
            };
            #endregion

            //до этого создали первый пустой FieldCustomView, поэтому плюсуем переменную-счетчик
            FieldCustomView.score++;

            #region Свайп
            SwipeGestureRecognizer swipeGesture = new SwipeGestureRecognizer()
            {
                Direction = SwipeDirection.Right
            };
            swipeGesture.Swiped += (s, e) => (App.Current.MainPage as MainPage).IsPresented = true;
            main.GestureRecognizers.Add(swipeGesture);
            #endregion

            scr = new ScrollView()
            {
                Content = main
            };

            Content = scr;
        }

        //запускает ряд методов по сохранению информации
        private async void SaveInfo(object mod)
        {
            add.IsEnabled = false;
            DbMenuListModel dbMenuListModel = mod as DbMenuListModel;

            //сюда будут добавляться ключи при парсинге
            List<KeysAndTypes> keysAndTypes = new List<KeysAndTypes>();

            bool DatabaseIsOk = false;

            bool response = await DisplayAlert("Вы уверены?",
                "Пожалуйста, проверьте все введенные данные", "Продолжить", "Проверить данные");

            if (response)
            {
                //пробуем спарсить введенные данные, если какие-то ошибки, то вернет false
                DatabaseIsOk = await ParseDataFromView(dbMenuListModel, keysAndTypes);
            }

            if (DatabaseIsOk)
            {
                //редактирование внеш. вида Detail - удаление большого "создать" 
                //и добавление View с listview
                MenuPage menuPage = (App.Current.MainPage as MainPage).Master as MenuPage;
                if (menuPage.menuPageViewModel.DbList.Count == 0)
                {
                    menuPage.field2.Children.Remove(menuPage.emptyDBView2);
                    menuPage.field2.Children.Add(menuPage.field3);
                }

                if (Device.RuntimePlatform != Device.UWP)
                {
                    dbMenuListModel.IsLoading = "false";
                    dbMenuListModel.IsLoadingId = menuPage.menuPageViewModel.DbList.Count;
                    menuPage.menuPageViewModel.DbList.Add(dbMenuListModel);
                }

                NavigationPage np = new NavigationPage(new CreateBasePage())
                {
                    BarBackgroundColor = Color.FromHex("#d83434"),
                    BarTextColor = Color.White
                };

                (App.Current.MainPage as MainPage).Detail = np;

                if (Device.RuntimePlatform == Device.Android)
                {
                    await Task.Delay(100);
                }

                if (Device.RuntimePlatform != Device.UWP)
                {
                    (App.Current.MainPage as MainPage).IsPresented = true;
                }

                Task task = Task.Run(() => SaveAllInfoAsync(dbMenuListModel, keysAndTypes));
            }

            add.IsEnabled = true;
        }

        //парсит данные со страницы
        private async Task<bool> ParseDataFromView(DbMenuListModel dbMenuListModel, List<KeysAndTypes> keysAndTypes)
        {
            if (main.Children.Count != 0)
            {
                foreach (var item in main.Children)
                {
                    if (item is FieldCustomView)
                    {
                        FieldCustomView field = item as FieldCustomView;

                        string type = "";

                        if (field.picker.Text != null)
                        {
                            switch (field.picker.Text)
                            {
                                case "Текст":
                                    type = "Текст";
                                    break;
                                case "Номер телефона":
                                    type = "Номер телефона";
                                    break;
                                case "Число":
                                    type = "Число";
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            await DisplayAlert("Возникла проблема", "Не указан тип данных", "ОК");
                            return false;
                        }

                        int problem = 0;
                        foreach (var compareItem in main.Children)
                        {
                            if (compareItem is FieldCustomView)
                            {
                                FieldCustomView field2 = compareItem as FieldCustomView;

                                if (field.name.Text == field2.name.Text)
                                {
                                    problem++;

                                    if (problem == 2)
                                    {
                                        await DisplayAlert("Возникла проблема", "Имена полей не должны совпадать", "ОК");
                                        return false;
                                    }
                                }
                            }
                        }

                        if (problem == 1)
                        {
                            keysAndTypes.Add(new KeysAndTypes(field.name.Text, type)
                            {
                                DatabaseData = dbMenuListModel.DatabaseData
                            });
                        }
                        else if (problem == 2)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        //добавление себя и других приглашенных в базу
        private async void SaveDatabaseAndAddInvitedUser(string path, DbMenuListModel dbMenuListModel)
        {
            using (ApplicationContext db = new ApplicationContext(path))
            {
                db.DatabasesList.Add(dbMenuListModel);

                db.SaveChanges();

                //добавление к себе
                var thisAccount = db.Accounts.FirstOrDefault(p => p.Email == App.Gmail);

                if (thisAccount != null)
                {
                    thisAccount.UsersDatabases.Add(new UsersDatabases()
                    {
                        DbMenuListModel = dbMenuListModel,
                        DbAccountModel = thisAccount
                    });
                }

                db.SaveChanges();

                //добавление ко всем остальным, кто есть в списке приглашенных
                if (dbMenuListModel.IsPrivate && dbMenuListModel.InvitedUsers.Count > 0)
                {
                    foreach (var userInt in dbMenuListModel.InvitedUsers)
                    {
                        var obj = (from account in db.Accounts.Include(a => a.UsersDatabases)
                                   where account.Number == userInt
                                   select account).FirstOrDefault();

                        var database = (from databases in db.DatabasesList.Include(a => a.UsersDatabases)
                                        where databases.Id == dbMenuListModel.Id
                                        select databases).FirstOrDefault();

                        bool isEmpty = true;

                        if (obj != null)
                        {
                            foreach (var user in database.UsersDatabases)
                            {
                                if (user.DbAccountModelEmail == obj.Email)
                                {
                                    isEmpty = false;
                                    break;
                                }
                            }

                            if (isEmpty)
                            {
                                obj.UsersDatabases.Add(new UsersDatabases()
                                {
                                    DbMenuListModel = dbMenuListModel,
                                    DbAccountModel = obj
                                });
                            }
                        }
                    }
                }

                await db.SaveChangesAsync();

                Device.BeginInvokeOnMainThread(() =>
                {
                    if (!(App.Current.MainPage as MainPage).IsPresented)
                    {
                        (App.Current.MainPage as MainPage).IsPresented = true;
                    }
                });
            }
        }
        
        //добавляет базу в ListView
        private void AddNewBaseToListViewInDetail(DbMenuListModel dbMenuListModel)
        {
            MenuPage menuPage = (App.Current.MainPage as MainPage).Master as MenuPage;

            dbMenuListModel.IsLoading = "true";

            if (Device.RuntimePlatform == Device.UWP)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    menuPage.menuPageViewModel.DbList.Add(dbMenuListModel);
                });
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    menuPage.menuPageViewModel.DbList.RemoveAt(dbMenuListModel.IsLoadingId);
                    menuPage.menuPageViewModel.DbList.Add(dbMenuListModel);
                });         
            }
        }

        //сохранение данных
        private void SaveAllInfoAsync(DbMenuListModel dbMenuListModel, List<KeysAndTypes> keysAndTypes)
        {
            dbMenuListModel.DatabaseData.DatabaseName = dbMenuListModel.Name;
            dbMenuListModel.DatabaseData.Data = keysAndTypes;

            var path = DependencyService.Get<IPathDatabase>().GetDataBasePath("ok3.db");

            //сохраняем базу и добавляем в неё приглашенных пользователей
            SaveDatabaseAndAddInvitedUser(path, dbMenuListModel);

            //добавление базы в ListView
            AddNewBaseToListViewInDetail(dbMenuListModel);
        }

        //нажатие на picker-Entry
        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            (sender as Entry).Unfocus();

            string x = await DisplayActionSheet("Укажите тип данных", "", "", "Текст", "Число", "Номер телефона");

            (sender as Entry).Text = x;
        }

        //нажатие кнопки "Удалить"
        private async void DeleteBtnReleaseV2(object sender, EventArgs e)
        {
            bool x = await DisplayAlert("Удаление поля", "Вы точно хотите удалить поле?", "Да", "Отмена");

            if (x)
            {
                int getId = Convert.ToInt32(((sender as Button).Parent.Parent.Parent.Parent as FieldCustomView).ClassId);

                //переопределение вызовет метод Callback
                ((sender as Button).Parent.Parent.Parent.Parent.Parent.Parent.Parent as BaseConstructorPage).ButtonId = getId;

                FieldCustomView.score--;
            }
        }

        //удаляет Frame и переназначает все Id-шники для в объектов в стэке
        //так как возможно, что Frame удалился из середины и Id-шники теперь идут
        //не по порядку
        private void Callback()
        {
            main.Children.RemoveAt(ButtonId);

            int newScore = 0;

            foreach (var item in main.Children)
            {
                if (item is FieldCustomView)
                {
                    if (newScore == 0)
                    {
                        (item as FieldCustomView).mField.IsVisible = true;
                    }
                    item.ClassId = newScore.ToString();
                    newScore++;
                }
            }
        }
    }
}
