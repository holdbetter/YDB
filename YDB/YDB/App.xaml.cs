using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YDB.Views;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace YDB
{
    public partial class App : Application
    {
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
