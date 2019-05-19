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
using Microsoft.EntityFrameworkCore;

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
            //в UwpLoginRequest / MenuPageViewModel открывается форма с логином
            //после ввода данных гугл возвращает Uri в который, есть code - auth_code
            //этот Uri попадает в свойство Uri данного объекта
            if (Uri != null && prop == "Uri" && Uri.Contains("code=") == true)
            {
                #region Парсим code из Uri
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

                //если получение провалилось и десериализация не клеится, то выдаст DisplayAlert
                if (googleProfile != null && tokenModel != null)
                {
                    DbAccountModel dbAccountModel = new DbAccountModel()
                    {
                        Email = googleProfile.Emails[0].Value,
                        GoogleNumbers = googleProfile.Id,
                        TokenInfo = tokenModel
                    };

                    #region Добавление гугл-профиля в базу данных и хранение в приложении
                    var path = DependencyService.Get<IPathDatabase>().GetDataBasePath("ok2.db");

                    using (ApplicationContext db = new ApplicationContext(path))
                    {
                        dbAccountModel.Number = db.Accounts.Count() + 1;

                        //если в базе такой акк уже есть, тогда не добавляем, а просто
                        //обновляем Current.Properties, а если нет такого пользователя,
                        //то добавляем в базу и ставив Properties
                        if (db.Accounts.FirstOrDefault(p => p.Email == dbAccountModel.Email) == null)
                        {
                            //тут коммент
                            //App.Gmail = dbAccountModel.Email;
                            Application.Current.Properties.Add("Email", dbAccountModel.Email);
                            Application.Current.Properties.Add("Expires", dbAccountModel.TokenInfo.DateTime.AddMonths(1));
                            await App.Current.SavePropertiesAsync();

                            db.Accounts.Add(dbAccountModel);
                            db.SaveChanges();
                        }
                        else if (db.Accounts.FirstOrDefault(p => p.Email == dbAccountModel.Email) != null)
                        {
                            var acc = db.Accounts.Include(a => a.TokenInfo).FirstOrDefault(p => p.Email == dbAccountModel.Email);

                            App.Gmail = dbAccountModel.Email;

                            if (acc.TokenInfo.DateTime < DateTime.UtcNow)
                            {
                                Application.Current.Properties.Add("Expires", dbAccountModel.TokenInfo.DateTime);
                                await App.Current.SavePropertiesAsync();

                                acc.TokenInfo = dbAccountModel.TokenInfo;
                                db.SaveChanges();
                            }
                        }
                    }
                    #endregion

                    menu.helloName.Text = "Привет!\n" + googleProfile.Emails[0].Value;
                    menu.scr1.Content = menu.field2;
                    menu.Content = menu.scr1;
                }                
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
