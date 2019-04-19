using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using YDB.Services;
using Xamarin.Forms;
using Plugin.CurrentActivity;
using YDB.Droid;
using Android.Support.Design.Widget;

[assembly: Xamarin.Forms.Dependency(typeof(SnackBarShow))]
namespace YDB.Droid
{
    public class SnackBarShow : ISnackBar
    {
        public void ShowSnackMessage()
        {
            //Activity activity = CrossCurrentActivity.Current.Activity;
            //Android.Views.View view = activity.FindViewById(Android.Resource.Id.Content);
            //Snackbar.Make(view, "Вы не вошли в аккаунт", Snackbar.LengthLong).Show();
        }
    }
}