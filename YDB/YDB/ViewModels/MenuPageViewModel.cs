using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using YDB.Views;
using YDB.Services;
using YDB.Models;
using System.Collections.ObjectModel;

namespace YDB.ViewModels
{
    public class MenuPageViewModel
    {
        public ICommand EnterInAppBtn { get; }
        public ICommand BaseCreateButton { get; }
        public ObservableCollection<DbMenuListModel> DbList { get; set; }

        public MenuPageViewModel()
        {
            DbList = new ObservableCollection<DbMenuListModel>();

            EnterInAppBtn = new Command(() =>
            {
                string authRequest = App.AuthorizeUrl + 
                    "?redirect_uri=" + App.RedirectUrl +
                    "&prompt=consent" +
                    "&response_type=code" +
                    "&client_id=" + App.ClientId +
                    "&scope=" + App.Scope;

                Device.OpenUri(new Uri(authRequest));
            });

            BaseCreateButton = new Command(async () =>
            {
                MainPage current = App.Current.MainPage as MainPage;

                NavigationPage page = (App.Current.MainPage as MainPage).Detail as NavigationPage;

                if (!(page.CurrentPage is CreateBasePage))
                {
                    current.Detail = new NavigationPage(new CreateBasePage())
                    {
                        //BarBackgroundColor = Device.RuntimePlatform != Device.UWP ? Color.FromHex("#d83434") : Color.Default
                        BarBackgroundColor = Color.FromHex("#d83434")
                    };

                    if (Device.RuntimePlatform == Device.Android)
                    {
                        await Task.Delay(100);
                    }
                }
                
                current.IsPresented = false;
            });
        }
    }
}
