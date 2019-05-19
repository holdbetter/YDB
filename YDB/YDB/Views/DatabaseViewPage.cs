using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;
using YDB.Services;
using YDB.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace YDB.Views
{
    //Page демонстрирует все объекты хранящиеся в таблице, отображая их главное поле 
    public class DatabaseViewPage : ContentPage
    {
        //текущий вид коллекции
        ObservableCollection<Values> mainValues;
        //хранит вид коллекции по умолчанию - такой, какой был создан (сорт по Id)
        ObservableCollection<Values> defaultView;

        public DatabaseViewPage(DbMenuListModel model)
        {
            BindingContext = model;
            this.SetBinding(TitleProperty, "Name");

            mainValues = new ObservableCollection<Values>();

            #region ViewSettings
            //Название главное поля
            Label mainKey = new Label()
            {
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(10, 0),
                BindingContext = model.DatabaseData.Data.Count > 0 ? model.DatabaseData.Data[0] : null,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.End,
                FontFamily = App.fontNameMedium,
                TextColor = Color.FromHex("#d83434"),
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) * 1
            };

            if (mainKey.BindingContext == null)
            {
                mainKey.Text = "Мы не нашли ни полей, ни значений!";
            }
            else
            {
                mainKey.SetBinding(Label.TextProperty, "Key");
            }

            #region FormattedStr для количество объектов и Label под этот текст

            FormattedString fs = new FormattedString();

            if (mainKey.BindingContext != null)
            {
                Span text = new Span()
                {
                    Text = "Количество объектов: ",
                };

                Span count = new Span()
                {
                    BindingContext = mainValues,
                };
                count.SetBinding(Span.TextProperty, "Count");
                fs.Spans.Add(text);
                fs.Spans.Add(count);
            }

            Label objectsCountLabel = new Label()
            {
                HorizontalOptions = LayoutOptions.EndAndExpand,
                FormattedText = fs,
                Margin = new Thickness(10, 0),
                BindingContext = model.DatabaseData.Data.Count > 0 ? model.DatabaseData.Data[0] : null,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center,
                FontFamily = App.fontNameRegular,
                TextColor = Color.FromHex("#d83434"),
                FontSize = Device.RuntimePlatform == Device.UWP ?
                           Device.GetNamedSize(NamedSize.Small, typeof(Label)) * 0.8 :
                           Device.GetNamedSize(NamedSize.Medium, typeof(Label)) * 0.8
            };

            #endregion

            //Стэк для фрейма с информацией о таблице
            StackLayout frameInfoStack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                Children = { mainKey, objectsCountLabel }
            };

            //Фрейм с информацией о таблице
            Frame mainInfo = new Frame()
            {
                Margin = new Thickness(12, 12),
                Padding = new Thickness(4, 4),
                CornerRadius = 14,
                BackgroundColor = Color.FromHex("#d83434"),
                Content = new Frame()
                {
                    CornerRadius = 11,
                    Padding = new Thickness(15, 15),
                    BackgroundColor = Color.White,
                    Content = new StackLayout()
                    {
                        Children =
                        {
                            frameInfoStack,
                        }
                    }
                }
            };

            #region Работа со списками
            //заполняем главный список объектами из главного поля
            if (model.DatabaseData.Data.Count > 0)
            {
                foreach (var item in model.DatabaseData.Data[0].Values)
                {
                    mainValues.Add(item);
                }
            }

            //заполняем список "по умолчанию" список объектами из главного поля
            defaultView = new ObservableCollection<Values>();
            foreach (var item in mainValues)
            {
                defaultView.Add(item);
            }
            #endregion

            #region Picker сортировок и все, что с ним связано
            //Пикер для сортировки
            Picker sortPick = new Picker()
            {
                Margin = new Thickness(5, 0),
                HorizontalOptions = LayoutOptions.Fill,
                Items = { "По умолчанию", "По алфавиту (А -> Я)", "По алфавиту (Я -> А)" },
                FontFamily = App.fontNameRegular
            };
            sortPick.SelectedIndex = 0;
            sortPick.SelectedIndexChanged += SortPick_SelectedIndexChanged;

            //если есть объекты в базе, то добавляем сортировку во Frame
            if (mainValues.Count > 0)
            {
                StackLayout framePickerStack = new StackLayout()
                {
                    Margin = new Thickness(0, 10, 0, 0),
                    Orientation = StackOrientation.Horizontal,
                    Children =
                    {
                        new Label
                        {
                            VerticalTextAlignment = TextAlignment.Center,
                            Margin = new Thickness(10, 0, 0, 0),
                            Text = "Сортировка:",
                            FontFamily = App.fontNameRegular
                        },
                        sortPick
                    }
                };

                ((mainInfo.Content as Frame).Content as StackLayout).
                    Children.Insert(1, framePickerStack);
            }
            #endregion

            //Список объектов с ручным сепаратором
            ListView listView = new ListView()
            {
                ItemsSource = mainValues,
                SeparatorVisibility = SeparatorVisibility.None,
                ItemTemplate = new DataTemplate(() =>
                {
                    Label label = new Label()
                    {
                        VerticalTextAlignment = TextAlignment.Center,
                        FontFamily = App.fontNameMedium,
                        TextColor = Color.FromHex("#d83434"),
                        FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label))
                    };
                    label.SetBinding(Label.TextProperty, "Value");

                    BoxView separatorUWP = new BoxView()
                    {
                        VerticalOptions = LayoutOptions.EndAndExpand,
                        Margin = Device.RuntimePlatform == Device.UWP ? new Thickness(0, 0, -20, -12) : new Thickness(0, 0, -10, -15),
                        HeightRequest = 0.5,
                        HorizontalOptions = LayoutOptions.Fill,
                        BackgroundColor = Device.RuntimePlatform == Device.UWP ? Color.FromHex("#3f3f3f") : Color.Red
                    };

                    ViewCell viewCell = new ViewCell()
                    {
                        View = new StackLayout()
                        {
                            Padding = Device.RuntimePlatform == Device.UWP ? new Thickness(20, 12) : new Thickness(10, 15),
                            Children =
                            {
                                label,
                                separatorUWP
                            }
                        }
                    };

                    return viewCell;
                })
            };
            listView.ItemSelected += ListView_ItemSelected;
            listView.ItemTapped += ListView_ItemTapped;
            #endregion

            Content = new StackLayout()
            {
                Children =
                {
                    mainInfo,
                    listView
                }
            };
        }

        //сортировка
        private void SortPick_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = (sender as Picker).SelectedIndex;

            switch (selectedIndex)
            {
                case 0:
                    (sender as Picker).HorizontalOptions = LayoutOptions.FillAndExpand;
                    (sender as Picker).HorizontalOptions = LayoutOptions.Fill;

                    if (defaultView != null)
                    {
                        mainValues.Clear();

                        foreach (var item in defaultView)
                        {
                            mainValues.Add(item);
                        }
                    }
                    break;
                case 1:
                    (sender as Picker).HorizontalOptions = LayoutOptions.FillAndExpand;
                    (sender as Picker).HorizontalOptions = LayoutOptions.Fill;

                    var result = from value in defaultView
                                 orderby value.Value
                                 select value;

                    mainValues.Clear();

                    foreach (var item in result.ToList())
                    {
                        mainValues.Add(item);
                    }
                    break;
                case 2:
                    (sender as Picker).HorizontalOptions = LayoutOptions.FillAndExpand;
                    (sender as Picker).HorizontalOptions = LayoutOptions.Fill;

                    var resultdescend = from value in defaultView
                                 orderby value.Value descending
                                 select value;


                    mainValues.Clear();

                    foreach (var item in resultdescend)
                    {
                        mainValues.Add(item);
                    }

                    break;
            }
        }

        //нажатие на элемент в списке объектов
        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            int id = (e.Item as Values).Id;

            DbMenuListModel mod = this.BindingContext as DbMenuListModel;

            await Navigation.PushAsync(new DatabaseObjectViewPage(mod, id));
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                ((ListView)sender).SelectedItem = null;
            }
        }
    }
}
