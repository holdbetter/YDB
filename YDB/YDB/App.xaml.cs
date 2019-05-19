﻿using System;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YDB.Models;
using YDB.Views;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace YDB
{
    public partial class App : Application
    {
        #region AppGoogleApiInfo
        public static string ClientId = "502847541706-pkqrpuul246ud4hdp524a1ae8bj00qki.apps.googleusercontent.com";
        public static string RedirectUrl = "com.googleusercontent.apps.502847541706-pkqrpuul246ud4hdp524a1ae8bj00qki:/oauth2redirect";

        public static string ClientIdUWP = "502847541706-nagh31q7mr5jvnrlu25kujfjt77mglo6.apps.googleusercontent.com";
        public static string RedirectUrlUWP = "https://google.com";
        public static string ClientSecretUWP = "eQIje61NJbMZcZ5tC8JfbrKV";
        #endregion

        #region Links
        public static string Scope = "https://www.googleapis.com/auth/userinfo.email";
        public static string AuthorizeUrl = "https://accounts.google.com/o/oauth2/v2/auth";
        public static string AccessTokenUrl = "https://www.googleapis.com/oauth2/v4/token";
        #endregion

        #region Fonts
        public static readonly string fontNameBold = 
            Device.RuntimePlatform == Device.Android ? "GoogleSans-Bold.ttf#GoogleSans" :
            Device.RuntimePlatform == Device.iOS ? "GoogleSans-Bold" :
            Device.RuntimePlatform == Device.UWP ? "Assets/Fonts/GoogleSans-Bold.ttf#Google Sans" : null;

        public static readonly string fontNameMedium = 
            Device.RuntimePlatform == Device.Android ? "GoogleSans-Medium.ttf#GoogleSans" :
            Device.RuntimePlatform == Device.iOS ? "GoogleSans-Medium" :
            Device.RuntimePlatform == Device.UWP ? "Assets/Fonts/GoogleSans-Medium.ttf#Google Sans" : null;

        public static readonly string fontNameRegular = 
            Device.RuntimePlatform == Device.Android ? "GoogleSans-Regular.ttf#Google Sans" :
            Device.RuntimePlatform == Device.iOS ? "GoogleSans-Regular" :
            Device.RuntimePlatform == Device.UWP ? "Assets/Fonts/GoogleSans-Regular.ttf#Google Sans" : null;
        #endregion

        public static string Gmail = "";
        public static DateTime ExpiredTime;
        public static DbAccountModel Account;

        public App()
        {
            InitializeComponent();

            //Вытаскиваем Email, если есть
            if (Application.Current.Properties.ContainsKey("Email"))
            {
                App.Gmail = Application.Current.Properties["Email"] as string;
                App.ExpiredTime = Convert.ToDateTime(Application.Current.Properties["Expires"]);

                if (ExpiredTime < DateTime.UtcNow) //проверка на сервере должна быть
                {
                    App.Gmail = ""; //не загрузит профиль
                }
                else
                {
                    //запрос на получение в список всех доступных бд
                }
            }

            MainPage = new MainPage();
            ((MainPage as MainPage).Detail as NavigationPage).BarBackgroundColor = Color.FromHex("#d83434");
            ((MainPage as MainPage).Detail as NavigationPage).BarTextColor = Color.White;
        }

        protected override void OnStart()
        {
            Debug.WriteLine("OnStart");
        }
        protected override void OnSleep()
        {
            Debug.WriteLine("OnSleep");
        }
        protected override void OnResume()
        {
            Debug.WriteLine("OnResume");
        }
    }
}
