using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using Xamarin.Forms;

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


        public BaseConstructorPage()
        {
            FieldCustomView.score = 0;
            Title = "Создайте поля";

            #region Toolbar

            ToolbarItem add = new ToolbarItem();
            add.Command = new Command(async () => {

                var dictionary = new Dictionary<string, Object>();

                for (int i = 0; i < main.Children.Count - 1; i++) //last is Add button
                {
                    FieldCustomView frame = main.Children[0] as FieldCustomView;

                    Type type = null;

                    switch (frame.picker.Text)
                    {
                        case "Текст":
                            type = typeof(string);
                            break;
                        case "Число":
                            type = typeof(int);
                            break;
                        case "Номер телефона":
                            type = typeof(int);
                            break;
                    }

                    dictionary.Add(frame.name.Text, type);
                }

                await Navigation.PushAsync(new Page());

            });
            add.Order = ToolbarItemOrder.Primary;

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
                        }),
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        CornerRadius = 5
                    }
                }
            };

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
                FieldCustomView.score--;
                int get = Convert.ToInt32(((sender as Button).Parent.Parent.Parent as FieldCustomView).ClassId);
                ((sender as Button).Parent.Parent.Parent.Parent.Parent.Parent as BaseConstructorPage).ButtonId = get;
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
