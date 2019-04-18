﻿using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace YDB.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        private void SwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e)
        {
            (App.Current.MainPage as MainPage).IsPresented = true;
        }
    }
}