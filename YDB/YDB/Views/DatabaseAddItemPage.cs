using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YDB.Models;
using Xamarin.Forms;
using YDB.Services;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace YDB.Views
{
    public class DatabaseAddItemPage : ContentPage, INotifyPropertyChanged
    {
        private string currentIdOfObject;
        private string lastIdObject;

        public StackLayout main;
        ScrollView scr;
        Button add, save, delete;

        //Список "объектов", где "объект" набор ключ + значение для элемента
        List<DataObjectCreate> ListDataObject = new List<DataObjectCreate>();

        public string CurrentIdOfObject
        {
            get
            {
                return currentIdOfObject;
            }
            set
            {
                currentIdOfObject = value;
                OnPropertyChanged("CurrentIdOfObject");
            }
        } //текущая "страничка"-"объект"
        public string LastIdObject
        {
            get
            {
                return lastIdObject;
            }
            set
            {
                lastIdObject = value;
                OnPropertyChanged("LastIdObject");
            }
        } //последняя возможная "страничка"-"объект"

        public DatabaseAddItemPage(DbMenuListModel model)
        {
            BindingContext = model;

            this.SetBinding(TitleProperty, "Name");

            main = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            #region ViewSettings + создание "объектов"
            //если есть какие-то ключи, то можно создавать объекты
            if (model.DatabaseData.Data.Count > 0)
            {
                #region Toolbar
                ToolbarItem toolbarItem = new ToolbarItem();
                toolbarItem.Command = new Command(SaveValues);
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

                //если есть какие-то ключи и есть запись в первом ключе,
                //тогда можно загрузить такой объект
                if (model.DatabaseData.Data[0].Values.Count != 0)
                {
                    //создаем объекты, если значение в первом ключе не пустое 
                    //(оно не может быть пустым, если оно есть)
                    //"объект" в данном случае это кусок View c всеми Value привязанным к ключу по Id
                    //DataObjectCreate - одна страница с объектом
                    int index = 0;
                    foreach (var item in model.DatabaseData.Data[0].Values)
                    {
                        DataObjectCreate dataObjectCreate = new DataObjectCreate(this.BindingContext as DbMenuListModel, index);
                        ListDataObject.Add(dataObjectCreate);
                        index++;
                    }
                }
            }
            else //если нет полей
            {
                Label label = new Label()
                {
                    Text = "У вас не добавлены поля, поэтому добавлять пока нечего.",
                    TextColor = Color.Gray,
                    FontFamily = App.fontNameMedium,
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                    HorizontalTextAlignment = TextAlignment.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.CenterAndExpand
                };
                main.Children.Add(label);
            }

            //добавление собранных "объектов" на страницу
            if (model.DatabaseData.Data.Count > 0)
            {
                if (ListDataObject.Count > 0)
                {
                    //добавляем во View первый объект
                    main.Children.Add(ListDataObject[0]);
                    CurrentIdOfObject = 1.ToString();
                    LastIdObject = ListDataObject.Count.ToString();
                }
                else
                {
                    //если объектов не было, но поля есть, тогда инициализируем пустой объект
                    DataObjectCreate dataObjectCreate = new DataObjectCreate(this.BindingContext as DbMenuListModel, -1);
                    ListDataObject.Add(dataObjectCreate);
                    main.Children.Add(ListDataObject[0]);
                    CurrentIdOfObject = 1.ToString();
                    LastIdObject = ListDataObject.Count.ToString();
                }

                #region Нижняя панель управления
                Button forward = new Button()
                {
                    Margin = new Thickness(0, 0, 10, 0),
                    BackgroundColor = Color.FromHex("#d83434"),
                    Text = "Вперед",
                    TextColor = Color.White,
                    FontFamily = App.fontNameMedium,
                    Command = new Command(GoForward),
                    CommandParameter = model,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center
                };

                Button backward = new Button()
                {
                    Margin = new Thickness(10, 0, 0, 0),
                    BackgroundColor = Color.FromHex("#d83434"),
                    Text = "Назад",
                    TextColor = Color.White,
                    FontFamily = App.fontNameMedium,
                    Command = new Command(GoBackward),
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center
                };

                Span now = new Span()
                {
                    TextColor = Color.White,
                    FontFamily = App.fontNameMedium,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    BindingContext = this
                };
                now.SetBinding(Span.TextProperty, "CurrentIdOfObject");

                Span slash = new Span()
                {
                    TextColor = Color.White,
                    FontFamily = App.fontNameMedium,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    Text = " / "
                };

                Span max = new Span()
                {
                    TextColor = Color.White,
                    FontFamily = App.fontNameMedium,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    BindingContext = this
                };
                max.SetBinding(Span.TextProperty, "LastIdObject");

                FormattedString formattedString = new FormattedString();
                formattedString.Spans.Add(now);
                formattedString.Spans.Add(slash);
                formattedString.Spans.Add(max);

                Label scorer = new Label()
                {
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    FormattedText = formattedString
                };

                StackLayout manage = new StackLayout()
                {
                    VerticalOptions = LayoutOptions.EndAndExpand,
                    BackgroundColor = Color.FromHex("#d83434"),
                    Orientation = StackOrientation.Horizontal,
                    Children =
                    {
                        backward, scorer, forward
                    }
                };

                add = new Button()
                {
                    BorderWidth = 1.5,
                    BorderColor = Color.FromHex("#d83434"),
                    BackgroundColor = Color.White,
                    Text = "Добавить",
                    TextColor = Color.FromHex("#d83434"),
                    FontFamily = App.fontNameMedium,
                    Command = new Command(AddNewObject),
                    CommandParameter = model,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    CornerRadius = 5
                };

                save = new Button()
                {
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

                delete = new Button()
                {
                    BorderWidth = 1.5,
                    BorderColor = Color.FromHex("#d83434"),
                    BackgroundColor = Color.White,
                    Text = " Удалить ",
                    TextColor = Color.FromHex("#d83434"),
                    FontFamily = App.fontNameMedium,
                    Command = new Command(DeleteObject),
                    CommandParameter = model,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    CornerRadius = 5
                };

                StackLayout buttons = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Padding = new Thickness(5, 0),
                    Children =
                    {
                        delete, save, add
                    }
                };

                StackLayout end = new StackLayout()
                {
                    VerticalOptions = LayoutOptions.EndAndExpand,
                    Children =
                    {
                        buttons,
                        manage
                    }
                };
                main.Children.Add(end);
                #endregion
            }
            #endregion

            Content = main;
        }

        private void GoForward()
        {
            int min = Convert.ToInt32(CurrentIdOfObject);
            int max = Convert.ToInt32(LastIdObject);

            if (min + 1 <= max)
            {
                main.Children[0] = ListDataObject[min];
                min++;
                CurrentIdOfObject = min.ToString();
            }
        }

        private void GoBackward()
        {
            int min = Convert.ToInt32(CurrentIdOfObject);
            int max = Convert.ToInt32(LastIdObject);

            if (min - 2 >= 0)
            {
                main.Children[0] = ListDataObject[min - 2];
                min--;
                CurrentIdOfObject = min.ToString();
            }
        }

        private void AddNewObject(object obj)
        {
            DataObjectCreate dataObjectCreate = new DataObjectCreate(obj as DbMenuListModel, -1);
            ListDataObject.Add(dataObjectCreate);
            main.Children[0] = dataObjectCreate;
            CurrentIdOfObject = ListDataObject.Count.ToString();
            LastIdObject = ListDataObject.Count.ToString();
        }

        private async void SaveValues(object mod)
        {
            bool ok = true;

            var model = mod as DbMenuListModel;

            var path = DependencyService.Get<IPathDatabase>().GetDataBasePath("ok3.db");

            if (ok)
            {
                using (ApplicationContext db = new ApplicationContext(path))
                {
                    var obj = (from database in db.DatabasesList
                              .Include(m => m.DatabaseData).ThenInclude(ub => ub.Data).ThenInclude(ub => ub.Values)
                              .Include(m => m.UsersDatabases)
                              .ToList()
                              where database.Id == model.Id
                              select database).FirstOrDefault();

                    foreach (var dataObj in ListDataObject)
                    {
                        var stackChildren = ((dataObj.Children[0] as ScrollView).Children[0] as StackLayout).Children;

                        int y = 0;
                        bool isEmpty = false;
                        for (int i = 0; i < stackChildren.Count; i++)
                        {
                            if (stackChildren[i] is TableItemOnAdd)
                            {
                                if ((stackChildren[i] as TableItemOnAdd).Index == -1) //новый объект
                                {
                                    if (i == 0)
                                    {
                                        if ((stackChildren[0] as TableItemOnAdd).value.Text == null ||
                                            (stackChildren[0] as TableItemOnAdd).value.Text == "")
                                        {
                                            isEmpty = true;
                                            ok = false;
                                            break;
                                        }
                                    }

                                    if ((stackChildren[i] as TableItemOnAdd).value.Text == null)
                                    {
                                        (stackChildren[i] as TableItemOnAdd).value.Text = "";
                                    }

                                    string value = (stackChildren[i] as TableItemOnAdd).value.Text;
                                    obj.DatabaseData.Data[y].Values.Add(new Values { Value = value });

                                    y++;
                                }
                                else //старый объект - обновление данных
                                {
                                    if (i == 0)
                                    {
                                        if ((stackChildren[0] as TableItemOnAdd).value.Text == null ||
                                            (stackChildren[0] as TableItemOnAdd).value.Text == "")
                                        {
                                            isEmpty = true;
                                            ok = false;
                                            break;
                                        }
                                    }

                                    if ((stackChildren[i] as TableItemOnAdd).value.Text == null)
                                    {
                                        (stackChildren[i] as TableItemOnAdd).value.Text = "";
                                    }

                                    string value = (stackChildren[i] as TableItemOnAdd).value.Text;

                                    try
                                    {
                                        obj.DatabaseData.Data[y].
                                            Values[(stackChildren[i] as TableItemOnAdd).Index].Value = value;
                                    }
                                    catch(ArgumentOutOfRangeException)
                                    {
                                        obj.DatabaseData.Data[y].
                                            Values.Add(new Values { Value = value });
                                    }

                                    y++;
                                }
                                
                            }
                        }

                        if (isEmpty == true)
                        {
                            await DisplayAlert("Проблема",
                                        "Главное поле не может быть пустым\n" +
                                        "Проверьте, все ли первые поля заполнены", "ОК");
                            break;
                        }
                    }

                    if (ok)
                    {
                        DatabaseMenuPage.model = obj;
                        db.SaveChanges();
                    }
                }
            }

            if (ok)
            {
                await Navigation.PopAsync();
            }
        }

        private async void DeleteObject(object mod)
        {
            int id = Convert.ToInt32(currentIdOfObject);

            if (id == 0)
            {
                await DisplayAlert("Ошибка", "Нечего удалять", "ОК");
                return;
            }

            bool ok = await DisplayAlert("Удаление объекта", "Вы действительно хотите удалить объект?\n" +
                "Это действие нельзя отменить", "ОК", "Отмена");

            if (ok)
            {
                DbMenuListModel model = mod as DbMenuListModel;

                var path = DependencyService.Get<IPathDatabase>().GetDataBasePath("ok3.db");
                using (ApplicationContext db = new ApplicationContext(path))
                {
                    var obj = (from database in db.DatabasesList
                                  .Include(m => m.DatabaseData).ThenInclude(ub => ub.Data).ThenInclude(ub => ub.Values)
                                  .Include(m => m.UsersDatabases)
                                  .ToList()
                               where database.Id == model.Id
                               select database).FirstOrDefault();

                    Action DeleteValues = () =>
                    {
                        foreach (var key in obj.DatabaseData.Data)
                        {
                            int curId = Convert.ToInt32(currentIdOfObject);

                            if (key.Values.Count != 0)
                            {
                                key.Values.RemoveAt(curId - 1);
                            }
                        }
                    };

                    if (id == ListDataObject.Count)
                    {
                        if (ListDataObject.Count == 1)
                        {
                            if (ListDataObject[id - 1].Index != -1)
                            {
                                DeleteValues();
                            }

                            ListDataObject.RemoveAt(id - 1);

                            Label label = new Label()
                            {
                                Text = "У вас не осталось полей :(",
                                TextColor = Color.Gray,
                                FontFamily = App.fontNameMedium,
                                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                                HorizontalTextAlignment = TextAlignment.Center,
                                HorizontalOptions = LayoutOptions.Center,
                                VerticalOptions = LayoutOptions.CenterAndExpand
                            };

                            main.Children[0] = label;

                            LastIdObject = ListDataObject.Count.ToString();
                            CurrentIdOfObject = 0.ToString();
                        }
                        else
                        {
                            if (ListDataObject[id - 1].Index != -1)
                            {
                                DeleteValues();
                            }

                            ListDataObject.RemoveAt(id - 1);

                            main.Children[0] = ListDataObject[id - 2];

                            LastIdObject = ListDataObject.Count.ToString();
                            CurrentIdOfObject = (id - 1).ToString();
                        }
                    }
                    else if (id == 1 && ListDataObject.Count > 1)
                    {
                        if (ListDataObject[id - 1].Index != -1)
                        {
                            DeleteValues();
                        }

                        ListDataObject.RemoveAt(id - 1);

                        main.Children[0] = ListDataObject[id - 1];

                        LastIdObject = ListDataObject.Count.ToString();
                        CurrentIdOfObject = (id).ToString();
                    }
                    else
                    {
                        if (ListDataObject[id - 1].Index != -1)
                        {
                            DeleteValues();
                        }

                        ListDataObject.RemoveAt(id - 1);

                        main.Children[0] = ListDataObject[id - 1];
                        LastIdObject = ListDataObject.Count.ToString();
                        CurrentIdOfObject = (id).ToString();
                    }

                    DatabaseMenuPage.model = obj;
                    await db.SaveChangesAsync();
                }
            }
        }
    }
}