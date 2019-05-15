using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YDB.Models;
using Xamarin.Forms;
using YDB.Services;
using Microsoft.EntityFrameworkCore;

namespace YDB.Views
{
    public class DatabaseAddItemPage : ContentPage
    {
        public StackLayout main;
        ScrollView scr;
        Button save;

        public DatabaseAddItemPage(DbMenuListModel model)
        {
            BindingContext = model;

            this.SetBinding(TitleProperty, "Name");

            main = new StackLayout()
            {

            };

            foreach (var item in model.DatabaseData.Data)
            {
                TableItemOnAdd tableItem = new TableItemOnAdd(item);
                main.Children.Add(tableItem);
            }

            save = new Button()
            {
                Margin = new Thickness(5, 5),
                BorderWidth = 1.5,
                BorderColor = Color.FromHex("#d83434"),
                BackgroundColor = Color.White,
                Text = "Сохранить",
                TextColor = Color.FromHex("#d83434"),
                FontFamily = App.fontNameMedium,
                Command = new Command(SaveValues),
                CommandParameter = model,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                CornerRadius = 5
            };
            main.Children.Add(save);

            scr = new ScrollView()
            {
                Content = main
            };

            Content = scr;
        }

        private async void SaveValues(object mod)
        {
            bool ok = true;

            var model = mod as DbMenuListModel;

            var path = DependencyService.Get<IPathDatabase>().GetDataBasePath("ok2.db");

            //Значения не могут быть пустыми

            if (ok)
            {
                using (ApplicationContext db = new ApplicationContext(path))
                {
                    var obj = (from database in db.DatabasesList
                              .Include(m => m.DatabaseData).ThenInclude(ub => ub.Data)
                              .Include(m => m.UsersDatabases)
                              .ToList()
                              where database.Id == model.Id
                              select database).FirstOrDefault();

                    int y = 0;
                    for (int i = 0; i < main.Children.Count; i++)
                    {
                        if (main.Children[i] is TableItemOnAdd)
                        {
                            string value = (main.Children[i] as TableItemOnAdd).value.Text;

                            //добавление
                            obj.DatabaseData.Data[y].Values.Add(new Values { Value = value });

                            y++;
                        }
                    }

                    db.SaveChanges();
                }
            }

            if (ok)
            {
                Navigation.RemovePage(this);
            }
        }
    }
}