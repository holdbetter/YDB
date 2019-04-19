using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YDB.Views;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace YDB
{
    public partial class App : Application
    {
        public static string clientId = "502847541706-pkqrpuul246ud4hdp524a1ae8bj00qki.apps.googleusercontent.com";
        public static string Scope = "https://www.googleapis.com/auth/userinfo.email";
        public static string AuthorizeUrl = "https://accounts.google.com/o/oauth2/auth";
        public static string AccessTokenUrl = "https://www.googleapis.com/oauth2/v4/token";
        public static string UserInfoUrl = "https://www.googleapis.com/oauth2/v2/userinfo";
        public static string RedirectUrl = "com.googleusercontent.apps.502847541706-pkqrpuul246ud4hdp524a1ae8bj00qki:/oauth2redirect";
        //public static string AuthCode = "4/MAH3NWkqXvui1GT3bJGwuSLJDCCQH3bFv-1ERl0PuI9-rM-PbLYD1ntpkYYiWmtAEIimp3mTYR9OZm_mn-_vDzo";
        public static string test = "{com.googleusercontent.apps.502847541706-pkqrpuul246ud4hdp524a1ae8bj00qki:/oauth2redirect?state=wygspojndkvfwbtb&code=4/MAF2WapSrSuUUM8MDipVxDOHeQnL5-l-Zd0vKH_kYFyJN6d5Sa0uWOXzsjv05RIJ0Xls9ZLInO-lqKonBqxRIYs&scope=email https://www.googleapis.com/auth/userinfo.email openid&authuser=1&session_state=e009c1907ec745995a9e5279b522b427f60c8d82..a92f&prompt=consent}";

        public static string fontNameBold = Device.RuntimePlatform == Device.Android ? "GoogleSans-Bold.ttf#GoogleSans" :
            Device.RuntimePlatform == Device.iOS ? "GoogleSans-Bold" :
            Device.RuntimePlatform == Device.UWP ? "Assets/Fonts/GoogleSans-Bold.ttf#Google Sans" : null;

        public static string fontNameMedium = Device.RuntimePlatform == Device.Android ? "GoogleSans-Medium.ttf#GoogleSans" :
            Device.RuntimePlatform == Device.iOS ? "GoogleSans-Medium" :
            Device.RuntimePlatform == Device.UWP ? "Assets/Fonts/GoogleSans-Medium.ttf#Google Sans" : null;

        public static string fontNameRegular = Device.RuntimePlatform == Device.Android ? "GoogleSans-Regular.ttf#Google Sans" :
            Device.RuntimePlatform == Device.iOS ? "GoogleSans-Regular" :
            Device.RuntimePlatform == Device.UWP ? "Assets/Fonts/GoogleSans-Regular.ttf#Google Sans" : null;


        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
            ((MainPage as MainPage).Detail as NavigationPage).BarBackgroundColor = Color.FromHex("#d83434");
            //((MainPage as MainPage).Detail as NavigationPage).BarBackgroundColor = 
            //    Device.RuntimePlatform != Device.UWP ? Color.FromHex("#d83434") 
            //                                         : Color.Default;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
