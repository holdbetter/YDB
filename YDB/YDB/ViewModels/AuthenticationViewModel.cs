using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Xamarin.Forms;
using YDB.Views;
using YDB.Services;
using YDB.ViewModels;
using YDB.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http;
using System.Linq;

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
            if (Uri != null && prop == "Uri" && Uri.Contains("code=") == true)
            {
                #region Обработка auth кода
                int pFrom = Uri.IndexOf("code=") + "code=".Length;
                int pTo = Uri.LastIndexOf("&");

                string r = Uri.Substring(pFrom, pTo - pFrom);

                int pFTo = r.IndexOf("&");

                string code = r.Substring(0, pFTo);

                #endregion

                MenuPage menu = (App.Current.MainPage as MainPage).Master as MenuPage;

                menu.btnGo.IsVisible = false;
                menu.hello.Text = "Загрузка...";
                menu.youNotLogin.Text = "";

                //получение Token-информации
                TokenModel tokenModel = await GetTokenInfo(code);
                this.accessToken = tokenModel.Access_token;

                //получение гугл-профиля
                GoogleProfileModel googleProfile = await GetGoogleInfo(this.accessToken);

                if (googleProfile != null && tokenModel != null)
                {
                    DbAccountModel dbAccountModel = new DbAccountModel()
                    {
                        Email = googleProfile.Emails[0].Value,
                        GoogleNumbers = googleProfile.Id,
                        TokenInfo = tokenModel
                    };

                    #region Добавление гугл-профиля в базу данных

                    var path = DependencyService.Get<IPathDatabase>().GetDataBasePath("ok2.db");

                    //if (такого мейла в базе нет) => *СОЗДАНИЕ и добавление* 
                    //else if (если мейл есть, ноне все данные (кроме мейла) совпадают) => *редактирование*
                    //else |когда все совпадает| нич не делаем

                    App.Gmail = dbAccountModel.Email;

                    using (ApplicationContext db = new ApplicationContext(path))
                    {
                        db.Accounts.Add(dbAccountModel);
                        db.SaveChanges();
                    }

                    #endregion
                }

                //menu.emptyList.FontSize = 5;
                menu.helloName.Text = "Привет!\n" + googleProfile.Emails[0].Value;
                menu.scr1.Content = menu.field2;
                menu.Content = menu.scr1;
            }
            else if (Uri != null && prop == "Uri" && Uri.Contains("code=") == false)
            {
                MainPage current = App.Current.MainPage as MainPage;
                await current.DisplayAlert("Упс!", "Вы не вошли в аккаунт", "ОК");
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private async Task<TokenModel> GetTokenInfo(string code)
        {
            if (Device.RuntimePlatform == Device.UWP)
            {
                var request = App.AccessTokenUrl + "?code=" + code +
                "&client_id=" + App.ClientIdUWP +
                "&client_secret=" + App.ClientSecretUWP +
                "&redirect_uri=" + App.RedirectUrlUWP +
                "&grant_type=authorization_code";

                HttpClient httpClient = new HttpClient();

                HttpResponseMessage response = await httpClient.PostAsync(request, null);
                var json = await response.Content.ReadAsStringAsync();
                var tokenModel = JsonConvert.DeserializeObject<TokenModel>(json);

                return tokenModel;
            }
            else
            {
                var request = App.AccessTokenUrl + "?code=" + code +
                "&client_id=" + App.ClientId +
                "&client_secret=" +
                "&redirect_uri=" + App.RedirectUrl +
                "&grant_type=authorization_code";

                HttpClient httpClient = new HttpClient();

                HttpResponseMessage response = await httpClient.PostAsync(request, null);
                var json = await response.Content.ReadAsStringAsync();
                var tokenModel = JsonConvert.DeserializeObject<TokenModel>(json);

                return tokenModel;
            }
        }

        private async Task<GoogleProfileModel> GetGoogleInfo(string token)
        {
            if (Device.RuntimePlatform == Device.UWP)
            {
                HttpClient client = new HttpClient();

                string request = "https://www.googleapis.com/plus/v1/people/me" + "?access_token=" + token;

                HttpResponseMessage response = await client.GetAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                GoogleProfileModel profile = JsonConvert.DeserializeObject<GoogleProfileModel>(json);
                return profile;
            }
            else
            {
                HttpClient client = new HttpClient();

                string request = "https://www.googleapis.com/plus/v1/people/me" + "?access_token=" + token;

                HttpResponseMessage response = await client.GetAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                GoogleProfileModel profile = JsonConvert.DeserializeObject<GoogleProfileModel>(json);
                return profile;
            }

        }
    }
}
