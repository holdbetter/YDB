using System;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YDB.Views;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace YDB
{
    public partial class App : Application
    {
        //AppGoogleApiInfo
        public static string ClientId = "502847541706-pkqrpuul246ud4hdp524a1ae8bj00qki.apps.googleusercontent.com";
        public static string RedirectUrl = "com.googleusercontent.apps.502847541706-pkqrpuul246ud4hdp524a1ae8bj00qki:/oauth2redirect";

        //Links
        public static string Scope = "https://www.googleapis.com/auth/userinfo.email";
        public static string AuthorizeUrl = "https://accounts.google.com/o/oauth2/v2/auth";
        public static string AccessTokenUrl = "https://www.googleapis.com/oauth2/v4/token";

        //Fonts
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
