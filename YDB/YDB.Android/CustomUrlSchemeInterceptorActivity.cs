using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Graphics.Drawables;
using YDB.Views;

namespace YDB.Droid
{
    [Activity(Label = "Loading...", NoHistory = true, LaunchMode = LaunchMode.SingleTop)]
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataSchemes = new[] { "com.googleusercontent.apps.502847541706-pkqrpuul246ud4hdp524a1ae8bj00qki" },
        DataPath = "/oauth2redirect")]
    public class CustomUrlSchemeInterceptorActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ColorDrawable colorDrawable = new ColorDrawable(Android.Graphics.Color.Rgb(216, 52, 52));

            ActionBar act = ActionBar;
            act.SetBackgroundDrawable(colorDrawable); //NavBarColor
            this.Window.SetBackgroundDrawable(colorDrawable); //backGroundColor

            //Забираем андройдовское Uri и конвертируем
            global::Android.Net.Uri uri_android = Intent.Data;
            Uri uri_netfx = new Uri(uri_android.ToString());

            //Сохраняем значение Uri
            (App.Current.MainPage as MainPage).authentication.UriAuthData = uri_netfx.OriginalString;

            //Завершаем активити
            this.Finish();

            return;
        }
    }
}