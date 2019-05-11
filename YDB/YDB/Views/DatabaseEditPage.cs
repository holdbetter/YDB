using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using YDB.Models;
using YDB.Services;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace YDB.Views
{
    public class DatabaseEditPage : ContentPage
    {
        private int buttonId;

        Button add, save;
        StackLayout main;
        ScrollView scr;

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

        public DatabaseEditPage(DbMenuListModel model)
        {
            BindingContext = model;

            main = new StackLayout()
            {
                Padding = new Thickness(10, 5, 10, 0),
            };

            ToolbarItem saveTool = new ToolbarItem();
            saveTool.Command = new Command(SaveKeys);
            saveTool.CommandParameter = model;

            if (Device.RuntimePlatform == Device.UWP)
            {
                saveTool.Icon = "checkMark.png";
                saveTool.Text = "Новое поле";
            }
            else
            {
                saveTool.Icon = "checkMark.png";
            }

            ToolbarItems.Add(saveTool);

            FieldCustomView.score = 0;

            foreach (var item in model.DatabaseData.Data)
            {
                FieldCustomView fieldCustomView = new FieldCustomView(TapGestureRecognizer_Tapped, DeleteBtnReleaseV2);
                fieldCustomView.name.Text = item.Key;
                fieldCustomView.picker.Text = item.Type;
                main.Children.Add(fieldCustomView);
                FieldCustomView.score++;
            }

            add = new Button()
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
            };

            save = new Button()
            {
                Margin = new Thickness(5, 5),
                BorderWidth = 1.5,
                BorderColor = Color.FromHex("#d83434"),
                BackgroundColor = Color.White,
                Text = "Сохранить",
                TextColor = Color.FromHex("#d83434"),
                FontFamily = App.fontNameMedium,
                Command = new Command(SaveKeys),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                CornerRadius = 5
            };

            StackLayout horStack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children = { add, save }
            };

            main.Children.Add(horStack);

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
            bool x = await DisplayAlert("Удаление поля", 
                "Удаление поля удалит все данные под этим полем. \nЭто действие нельзя отменить.", 
                "Да", "Отмена");

            if (x)
            {
                int get = Convert.ToInt32(((sender as Button).Parent.Parent.Parent as FieldCustomView).ClassId);
                ((sender as Button).Parent.Parent.Parent.Parent.Parent.Parent as DatabaseEditPage).ButtonId = get;
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

        private async void SaveKeys(object mod)
        {
            DbMenuListModel model = mod as DbMenuListModel;

            var path = DependencyService.Get<IPathDatabase>().GetDataBasePath("ok2.db");

            using (ApplicationContext db = new ApplicationContext(path))
            {
                bool DatabaseIsOk = true;

                var obj = (from database in db.DatabasesList
                           .Include(m => m.DatabaseData).ThenInclude(ub => ub.Data)
                           .Include(m => m.UsersDatabases)
                           .ToList()
                           where database.Id == model.Id
                           select database).FirstOrDefault();

                List<KeysAndTypes> keysAndTypes = new List<KeysAndTypes>();

                //собираем все ключи и типы в список
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
                                keysAndTypes.Add(new KeysAndTypes(field.name.Text, type));
                            }
                        }
                    }
                }

                if (DatabaseIsOk)
                {
                    //обновляем значения
                    for (int i = 0; i < keysAndTypes.Count; i++)
                    {
                        if (i < obj.DatabaseData.Data.Count)
                        {
                            obj.DatabaseData.Data[i].Key = keysAndTypes[i].Key;
                            obj.DatabaseData.Data[i].Type = keysAndTypes[i].Type;
                        }

                        if (i >= obj.DatabaseData.Data.Count)
                        {
                            keysAndTypes[i].DatabaseData = obj.DatabaseData.Data[0].DatabaseData;
                            obj.DatabaseData.Data.Add(keysAndTypes[i]);
                        }
                    }

                    db.SaveChanges();
                    await Navigation.PopAsync();
                }
            }
        }
    }
}
