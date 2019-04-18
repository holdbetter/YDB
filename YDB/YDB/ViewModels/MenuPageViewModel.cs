using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using YDB.Views;
using YDB.Services;

namespace YDB.ViewModels
{
    class MenuPageViewModel
    {
        public ICommand EnterInAppBtn { get; }
        public ICommand BaseCreateButton { get; }
        public bool isAuthorized = true;

        public MenuPageViewModel()
        {
            EnterInAppBtn = new Command(async() =>
            {
                //тут будет логин
                MainPage current = App.Current.MainPage as MainPage;

                if (isAuthorized)
                {
                    current.Detail = new NavigationPage(new BaseShowPage())
                    {
                        //BarBackgroundColor = Device.RuntimePlatform != Device.UWP ? Color.FromHex("#d83434") : Color.Default
                        BarBackgroundColor = Color.FromHex("#d83434")
                    };

                    MenuPage menu = current.Master as MenuPage;
                    menu.scr.Content = menu.field2;
                    menu.Content = menu.scr;
                }
                else
                {
                    if (Device.RuntimePlatform == Device.Android)
                    {
                        DependencyService.Get<ISnackBar>().ShowSnackMessage();
                    }
                    else
                    {
                        await current.DisplayAlert("Упс!", "Вы не вошли в аккаунт", "ОК");
                    }
                }
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
