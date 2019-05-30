using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;
using YDB.Models;
using YDB.Services;

namespace YDB.Views
{
	public partial class CreateBasePage : ContentPage
	{
        Label infoL, safetyL, markersL, nonPublic;
        Entry name;
        StackLayout main, forSwitch, markersStack;
        Switch isPublic;
        ScrollView markerScroll;
        
        DbMenuListModel dbMenuListModel;

        List<Entry> entriesOfInvitedId = new List<Entry>();

        public CreateBasePage ()
		{
            //создается объект модели для будущего заполнения и добавления в БД
            dbMenuListModel = new DbMenuListModel() { Carrier = App.Gmail } ;

            Title = "Создание базы данных";

            #region Toolbar
            ToolbarItem toolbarItem = new ToolbarItem();
            toolbarItem.Command = new Command(async() => 
            {
                toolbarItem.IsEnabled = false;

                //обрезка имени базы и uppercase первой буквы
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
                                           "Название базы данных  не может иметь больше 12 символов",
                                           "ОК");
                        toolbarItem.IsEnabled = true;
                        return;
                    }

                    dbMenuListModel.Name = result;
                }

                //парсинг данных по доступу к базе
                dbMenuListModel.IsPrivate = isPublic.IsToggled;

                //добавление Id-шников, если база приватная
                if (dbMenuListModel.IsPrivate)
                {
                    List<int> usersId = new List<int>();

                    foreach (Entry entry in entriesOfInvitedId)
                    {
                        if (entry.Text != null && entry.Text != "")
                        {
                            //тут должна быть обрезка Id, если есть какие-то лишние символы

                            if (!usersId.Contains(Convert.ToInt32(entry.Text)))
                            {
                                usersId.Add(Convert.ToInt32(entry.Text));
                            }
                        }
                    }

                    //передача ID
                    dbMenuListModel.InvitedUsers = usersId;
                }

                //если все ок, то передаем данные на следующую страницу
                //если нет, то уведомляем
                if (dbMenuListModel.Marker != null && dbMenuListModel.Name != "" && dbMenuListModel.Name != null)
                {
                    await Navigation.PushAsync(new BaseConstructorPage(dbMenuListModel));
                }
                else
                {
                    await DisplayAlert("Кажется вы что-то забыли",
                        "Проверьте название базы и выбрали ли вы для неё маркер", "ОК");
                }

                toolbarItem.IsEnabled = true;
            });

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
            main = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };


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

            #region Свайп
            SwipeGestureRecognizer swipeGesture = new SwipeGestureRecognizer()
            {
                Direction = SwipeDirection.Right
            };
            swipeGesture.Swiped += (s, e) => (App.Current.MainPage as MainPage).IsPresented = true;
            main.GestureRecognizers.Add(swipeGesture);
            #endregion

            ScrollView scroll = new ScrollView()
            {
                Content = main
            };
            Content = scroll;
		}

        private static string GetCarrier(string gmail)
        {
            var path = DependencyService.Get<IPathDatabase>().GetDataBasePath("ok3.db");

            using (ApplicationContext db = new ApplicationContext(path))
            {
                return (from account in db.Accounts
                        where account.Email == gmail
                        select account).FirstOrDefault().Email;
            }
        }

        private void IsPublic_Toggled(object sender, ToggledEventArgs e)
        {
            //local function
            void CreateField()
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

        private void IsMarkedCheck()
        {
            foreach (MarkerCustomView item in markersStack.Children)
            {
                if (item.Marked)
                {
                    dbMenuListModel.Marker = item;
                }
            }
        }
    }
}