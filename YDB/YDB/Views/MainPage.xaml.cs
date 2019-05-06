using YDB.Models;
using YDB.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace YDB.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : MasterDetailPage
    {
        public AuthenticationViewModel authentication;

        public MainPage()
        {
            MasterBehavior = MasterBehavior.Popover;

            authentication = new AuthenticationViewModel();

            InitializeComponent();
        }
    }
}