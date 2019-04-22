using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using YDB.Models;

namespace YDB.Views
{
    public partial class NewItemPage : ContentPage
    {
        public WebView view;

        public NewItemPage(string request)
        {
            view = new WebView()
            {
                Source = request
            };

            Content = view;
        }
    }
}