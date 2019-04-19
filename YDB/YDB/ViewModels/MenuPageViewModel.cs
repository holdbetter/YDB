using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Auth;
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
            EnterInAppBtn = new Command(() =>
            {
                //тут будет логин

                #region LoginBtn

                var authenficator = new OAuth2Authenticator(
                    App.clientId,
                    null,
                    App.Scope,
                    new Uri(App.AuthorizeUrl),
                    new Uri(App.RedirectUrl), //redirect
                    new Uri(App.AccessTokenUrl),
                    null,
                    true);

                authenficator.Completed += Authenficator_Completed;
                authenficator.Error += OnAuthError;

                AuthenticationState.Authenticator = authenficator;

                var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
                presenter.Login(authenficator);

                #endregion
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

        private async void Authenficator_Completed(object sender, AuthenticatorCompletedEventArgs e)
        {
            if (sender is OAuth2Authenticator authenticator)
            {
                authenticator.Completed -= Authenficator_Completed;
                authenticator.Error -= OnAuthError;
            }

            MainPage current = App.Current.MainPage as MainPage;

            //if (AuthenticatorState.uri.Contains("code="))
            //{
            //    isAuthorized = true;

            //    int pFrom = AuthenticatorState.uri.IndexOf("code=") + "code=".Length;
            //    int pTo = AuthenticatorState.uri.LastIndexOf("&");

            //    string result = AuthenticatorState.uri.Substring(pFrom, pTo - pFrom);

            //    int pFTo = result.IndexOf("&");

            //    string code_id = result.Substring(0, pFTo);
            //}

            if (e.IsAuthenticated)
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
        }

        void OnAuthError(object sender, AuthenticatorErrorEventArgs e)
        {
            if (sender is OAuth2Authenticator authenticator)
            {
                authenticator.Completed -= Authenficator_Completed;
                authenticator.Error -= OnAuthError;
            }

            System.Diagnostics.Debug.WriteLine("Authentication error: " + e.Message);
        }
    }
}
