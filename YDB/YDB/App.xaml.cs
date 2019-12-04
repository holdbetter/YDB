using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YDB.Models;
using YDB.Services;
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
        public static string Scope = 
            "https://www.googleapis.com/auth/contacts " +
            "https://www.googleapis.com/auth/userinfo.email " +
            "https://www.googleapis.com/auth/userinfo.profile";

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
        public static string Id;

        private bool IsLoggedIn;

        public App()
        {
            InitializeComponent();

            List<DbMenuListModel> databases = new List<DbMenuListModel>();

            //Вытаскиваем Email, если есть
            if (Application.Current.Properties.ContainsKey("Email"))
            {
                //срок годности последнего токена
                var tokenValid = Convert.ToDateTime(Application.Current.Properties["Expires"]);

                if (tokenValid < DateTime.UtcNow) //проверка на сервере должна быть
                {
                    LogOut();
                }
                else
                {
                    //запрос на получение в список всех доступных бд
                    //databases = GetDatabases - request

                    App.Gmail = Application.Current.Properties["Email"] as string;
                    App.ExpiredTime = Convert.ToDateTime(Application.Current.Properties["Expires"]);
                    App.Id = Application.Current.Properties["Id"] as string;

                    var path = DependencyService.Get<IPathDatabase>().GetDataBasePath("ok3.db");

                    using (ApplicationContext db = new ApplicationContext(path))
                    {
                        var bases = db.Accounts.Include(us => us.UsersDatabases).ThenInclude(data => data.DbMenuListModel).Where(a => a.Email == App.Gmail).FirstOrDefault();

                        if (bases != null)
                        {
                            foreach (var item in bases.UsersDatabases)
                            {
                                item.DbMenuListModel.IsLoading = "true";
                                databases.Add(item.DbMenuListModel);
                            }
                        }
                    }

                    //тут анкоммент
                    IsLoggedIn = true;
                }
            }

            MainPage = new MainPage();
            (MainPage as MainPage).IsPresented = true;

            //загружаем базы в ListView
            if (IsLoggedIn)
            {
                var menuPage = ((MainPage as MainPage).Master as MenuPage);
                menuPage.spanGmail.Text = App.Gmail;
                menuPage.spId.Text = App.Id;

                menuPage.field2.Children.Remove(menuPage.emptyDBView2);
                menuPage.field2.Children.Add(menuPage.field3);
                menuPage.scr1.Content = menuPage.field2;

                if (databases.Count != 0)
                {
                    //тут анкоммент
                    foreach (var item in databases)
                    {
                        menuPage.menuPageViewModel.DbList.Add(item);
                    }
                }

                (MainPage as MainPage).IsPresented = true;
            }

            ((MainPage as MainPage).Detail as NavigationPage).BarTextColor = Color.White;
            ((MainPage as MainPage).Detail as NavigationPage).BarBackgroundColor = Color.FromHex("#d83434");
        }

        private async void LogOut()
        {
            if (MainPage != null)
            {
                ((MainPage as MainPage).Master as MenuPage).menuPageViewModel.DbList.Clear();
                ((MainPage as MainPage).Master as MenuPage).databaseListStack.Children.Clear();
            }

            Application.Current.Properties.Remove("Email");
            Application.Current.Properties.Remove("Expires");
            Application.Current.Properties.Remove("Id");

            await App.Current.SavePropertiesAsync();
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
