using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Xamarin.Forms;
using YDB.Views;
using YDB.ViewModels;
using YDB.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http;

namespace YDB.ViewModels
{
    public class AuthenticationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        string accessToken;
        string uri;

        public string Uri
        {
            get => uri;
            set
            {
                uri = value;
                OnPropertyChanged("Uri");
            }
        }

        public async void OnPropertyChanged(string prop = "")
        {
            if (prop == "Uri" && Uri.Contains("code=") == true)
            {
                int pFrom = Uri.IndexOf("code=") + "code=".Length;
                int pTo = Uri.LastIndexOf("&");

                string r = Uri.Substring(pFrom, pTo - pFrom);

                int pFTo = r.IndexOf("&");

                string code = r.Substring(0, pFTo);

                MenuPage menu = (App.Current.MainPage as MainPage).Master as MenuPage;

                menu.btnGo.IsVisible = false;
                menu.hello.Text = "Загрузка...";
                menu.youNotLogin.Text = "";

                this.accessToken = await GetAccessToken(code);
                menu.emptyList.FontSize = 5;
                menu.emptyList.Text = accessToken;
                menu.scr.Content = menu.field2;
                menu.Content = menu.scr;
            }
            else if (prop == "Uri" && Uri.Contains("code=") == false)
            {
                MainPage current = App.Current.MainPage as MainPage;
                await current.DisplayAlert("Упс!", "Вы не вошли в аккаунт", "ОК");
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private async Task<string> GetAccessToken(string code)
        {
            var request = App.AccessTokenUrl + "?code=" + code +
                "&client_id=" + App.ClientId +
                "&client_secret=" +
                "&redirect_uri=" + App.RedirectUrl +
                "&grant_type=authorization_code";

            HttpClient httpClient = new HttpClient();

            HttpResponseMessage response = await httpClient.PostAsync(request, null);
            var json = await response.Content.ReadAsStringAsync();
            var newpeople = JsonConvert.DeserializeObject<TokenModel>(json);

            return newpeople.Access_token;
        }
    }
}
