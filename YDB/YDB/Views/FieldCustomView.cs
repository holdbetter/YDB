using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace YDB.Views
{
    //объект-View для создания ключа и его типа
    public class FieldCustomView : ContentView
    {
        Frame main;
        StackLayout frameStack;
        public Entry name;
        public Entry picker;
        public Label mField;
        Button deleteBtn;

        public static int score = 0; //количество созданных полей

        //tap - делегат, который вызывает DisplayActionSheet для выбора типа данных
        //delete - делегат кнопки удаления
        public FieldCustomView(EventHandler<FocusEventArgs> tap, EventHandler delete)
        {
            //передаем в ClassId количество полей - понадобится для удаления Frame
            //из страницы по Id
            ClassId = score.ToString();

            #region ViewSettings
            main = new Frame
            {
                CornerRadius = 10,
                BackgroundColor = Color.FromHex("#d83434"),
                Margin = new Thickness(0, 5, 0, 0)
            };

            frameStack = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.FromHex("#d83434")
            };

            name = new Entry()
            {
                Placeholder = "Название поля",
                FontFamily = App.fontNameRegular,
                PlaceholderColor = Color.White,
                TextColor = Color.White,
                BackgroundColor = Device.RuntimePlatform == Device.UWP ? Color.Transparent : Color.Default
            };

            picker = new Entry()
            {
                FontFamily = App.fontNameRegular,
                Placeholder = "Тип данных",
                PlaceholderColor = Color.White,
                TextColor = Color.White,
                VerticalOptions = LayoutOptions.StartAndExpand,
                BackgroundColor = Device.RuntimePlatform == Device.UWP ? Color.Transparent : Color.Default
            };
            picker.Focused += tap;

            StackLayout tryToCreateMainField = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal
            };

            deleteBtn = new Button()
            {
                ClassId = score.ToString(),
                BorderWidth = 1.5,
                BorderColor = Color.White,
                Text = "Удалить",
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.EndAndExpand,
                VerticalOptions = LayoutOptions.End,
                BackgroundColor = Color.Transparent,
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Button)),
                FontFamily = App.fontNameMedium,
                CornerRadius = 5
            };

            mField = new Label()
            {
                Margin = new Thickness(10, 0, 0, 0),
                HorizontalOptions = LayoutOptions.Start,
                FontFamily = App.fontNameMedium,
                TextColor = Color.White,
                VerticalTextAlignment = TextAlignment.Center,
                Text = "Это главное поле"
            };

            tryToCreateMainField.Children.Add(mField);          
            tryToCreateMainField.Children.Add(deleteBtn);

            if (score != 0)
            {
                mField.IsVisible = false;
            }
            #endregion

            //события кнопки
            deleteBtn.Pressed += DeleteBtn_Pressed;
            deleteBtn.Released += DeleteBtn_Released;
            deleteBtn.Released += delete;

            frameStack.Children.Add(name);
            frameStack.Children.Add(picker);
            frameStack.Children.Add(tryToCreateMainField);

            //добавление стэка в Frame
            main.Content = frameStack;

            Content = main;
        }

        private void DeleteBtn_Released(object sender, EventArgs e)
        {
            (sender as Button).BackgroundColor = Color.Transparent;
            (sender as Button).TextColor = Color.White;
        }

        private void DeleteBtn_Pressed(object sender, EventArgs e)
        {
            (sender as Button).BackgroundColor = Color.White;
            (sender as Button).TextColor = Color.FromHex("#d83434");
        }
    }
}
