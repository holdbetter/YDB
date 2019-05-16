using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using Xamarin.Forms;
using YDB.Models;
using YDB.Services;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace YDB.Views
{
    public class BaseConstructorPage : ContentPage
    {
        public StackLayout main;
        ScrollView scr;

        int buttonId;
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

            ToolbarItem add = new ToolbarItem();
            add.Command = new Command(async () => 
            {
                add.IsEnabled = false;

                bool response = await DisplayAlert("Вы уверены?", 
                    "Пожалуйста, проверьте все введенные данные", "Продолжить", "Проверить данные");

                if (response)
                {
                    bool DatabaseIsOk = true;
                    MenuPage menuPage = (App.Current.MainPage as MainPage).Master as MenuPage;

                    //редактирование внеш. вида Detail - удаление большого "создать" 
                    //и добавление View с listview
                    if (menuPage.menuPageViewModel.DbList.Count == 0)
                    {
                        menuPage.field2.Children.Remove(menuPage.emptyDBView2);
                        menuPage.field2.Children.Add(menuPage.field3);
                    }

                    NavigationPage np = new NavigationPage(new CreateBasePage())
                    {
                        BarBackgroundColor = Color.FromHex("#d83434"),
                        BarTextColor = Color.White
                    };

                    var path = DependencyService.Get<IPathDatabase>().GetDataBasePath("ok2.db");

                    using (ApplicationContext db = new ApplicationContext(path))
                    {
                        //var list = db.Accounts.Include(p => p.DbMenuListModels).ToList();

                        List<KeysAndTypes> keysAndTypes = new List<KeysAndTypes>();

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
                                        DatabaseIsOk = false;
                                        await DisplayAlert("Возникла проблема", "Не указан тип данных", "ОК");
                                        break;
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
                                                    DatabaseIsOk = false;
                                                    await DisplayAlert("Возникла проблема", "Имена полей не должны совпадать", "ОК");
                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    if (problem == 1)
                                    {
                                        keysAndTypes.Add(new KeysAndTypes(field.name.Text, type) {
                                            DatabaseData = dbMenuListModel.DatabaseData });
                                    }
                                    else if (problem == 2)
                                    {
                                        break;
                                    }
                                }
                            }
                        }

                        if (DatabaseIsOk)
                        {
                            dbMenuListModel.DatabaseData.DatabaseName = dbMenuListModel.Name;
                            dbMenuListModel.DatabaseData.Data = keysAndTypes;

                            db.DatabasesList.Add(dbMenuListModel);

                            //добавление к себе
                            var thisAccount = db.Accounts.FirstOrDefault(p => p.Email == App.Gmail);

                            if (thisAccount != null)
                            {
                                thisAccount.UsersDatabases.Add(new UsersDatabases()
                                {
                                    DbMenuListModel = dbMenuListModel,
                                    DbAccountModel = thisAccount
                                });

                                db.SaveChanges();
                            }

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

                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }

                            db.SaveChanges();
                        }
                    }

                    //добавление базы в ListView
                    if (DatabaseIsOk)
                    {
                        menuPage.menuPageViewModel.DbList.Add(dbMenuListModel);

                        (App.Current.MainPage as MainPage).Detail = np;
                    }
                }

                add.IsEnabled = true;
            });

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
            FieldCustomView.score++;


            SwipeGestureRecognizer swipeGesture = new SwipeGestureRecognizer()
            {
                Direction = SwipeDirection.Right
            };
            swipeGesture.Swiped += (s, e) => (App.Current.MainPage as MainPage).IsPresented = true;
            main.GestureRecognizers.Add(swipeGesture);

            scr = new ScrollView()
            {
                Content = main
            };

            Content = scr;
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            (sender as Entry).Unfocus();

            string x = await DisplayActionSheet("Укажите тип данных", "", "", "Текст", "Число", "Номер телефона");

            (sender as Entry).Text = x;
        }

        private async void DeleteBtnReleaseV2(object sender, EventArgs e)
        {
            bool x = await DisplayAlert("Удаление поля", "Вы точно хотите удалить поле?", "Да", "Отмена");

            if (x)
            {
                int get = Convert.ToInt32(((sender as Button).Parent.Parent.Parent as FieldCustomView).ClassId);
                ((sender as Button).Parent.Parent.Parent.Parent.Parent.Parent as BaseConstructorPage).ButtonId = get;
                FieldCustomView.score--;
            }
        }

        private void Callback()
        {
            main.Children.RemoveAt(ButtonId);

            int newScore = 0;

            foreach (var item in main.Children)
            {
                item.ClassId = newScore.ToString();
                newScore++;
            }
        }
    }
}
