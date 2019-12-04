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
using System.Web;

namespace YDB.ViewModels
{
    public class AuthenticationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        string accessToken;
        string uri;

        public string UriAuthData
        {
            get => uri;
            set
            {
                uri = value;
                OnPropertyChanged("UriAuthData");
            }
        }

        public async void OnPropertyChanged(string prop = "")
        {
            //в UwpLoginRequest / MenuPageViewModel открывается форма с логином
            //после ввода данных гугл возвращает Uri в который, есть code - auth_code
            //этот Uri попадает в свойство Uri данного объекта
            if (UriAuthData != null && prop == "UriAuthData" && UriAuthData.Contains("code=") == true)
            {
                #region Парсим code из Uri

                string code = HttpUtility.ParseQueryString(new Uri(UriAuthData).Query).Get("code");

                //int pFrom = Uri.IndexOf("code=") + "code=".Length;
                //int pTo = Uri.LastIndexOf("&");
                //string code = Uri.Substring(pFrom, pTo - pFrom);
                //int pFTo = r.IndexOf("&");
                //string code = r.Substring(0, pFTo);

                #endregion

                MenuPage menu = (App.Current.MainPage as MainPage).Master as MenuPage;

                menu.btnGo.IsVisible = false;
                menu.hello.Text = "Загрузка...";
                menu.youNotLogin.Text = "";

                //получение Token-информации
                TokenModel tokenModel = await GetTokenInfo(code);
                this.accessToken = tokenModel.Access_token;

                //получение гугл-профиля
                GooglePersonModel googleProfile = await GetGoogleInfo(this.accessToken);

                //если получение провалилось и десериализация не клеится, то выдаст DisplayAlert
                if (googleProfile.EmailAddresses != null && tokenModel != null)
                {
                    DbAccountModel dbAccountModel = new DbAccountModel()
                    {
                        Email = googleProfile.EmailAddresses[0].Value,
                        GoogleNumbers = googleProfile.Id,
                        TokenInfo = tokenModel
                    };

                    #region Добавление гугл-профиля в базу данных и хранение в приложении
                    var path = DependencyService.Get<IPathDatabase>().GetDataBasePath("ok3.db");

                    using (ApplicationContext db = new ApplicationContext(path))
                    {
                        //если в базе такой акк уже есть, тогда не добавляем, а просто
                        //обновляем Current.Properties, а если нет такого пользователя,
                        //то добавляем в базу и ставив Properties

                        var tryGetExistingAccount = db.Accounts.Include(acc => acc.TokenInfo).FirstOrDefault(p => p.Email == dbAccountModel.Email);

                        if (tryGetExistingAccount == null)
                        {
                            //db.Accounts.Count должно возращаться и УВЕЛИЧИВАТЬСЯ на сервере
                            dbAccountModel.Number = db.Accounts.Count() + 1;

                            #region Обновление Properties
                            if (Application.Current.Properties.ContainsKey("Email"))
                            {
                                Application.Current.Properties.Remove("Email");
                                Application.Current.Properties.Remove("Expires");
                                Application.Current.Properties.Remove("Id");

                                Application.Current.Properties.Add("Email", dbAccountModel.Email);
                                Application.Current.Properties.Add("Expires", dbAccountModel.TokenInfo.DateTime);
                                Application.Current.Properties.Add("Id", dbAccountModel.Number.ToString());
                            }
                            else
                            {
                                Application.Current.Properties.Add("Email", dbAccountModel.Email);
                                Application.Current.Properties.Add("Expires", dbAccountModel.TokenInfo.DateTime);
                                Application.Current.Properties.Add("Id", dbAccountModel.Number.ToString());
                            }

                            await App.Current.SavePropertiesAsync();
                            #endregion

                            db.Accounts.Add(dbAccountModel);
                            db.SaveChanges();
                        }
                        else if (tryGetExistingAccount != null)
                        {
                            if (tryGetExistingAccount.TokenInfo.DateTime < DateTime.UtcNow)
                            {
                                tryGetExistingAccount.TokenInfo.Expires_in = dbAccountModel.TokenInfo.Expires_in;
                                db.SaveChanges();
                            }

                            #region Обновление Properties
                            if (Application.Current.Properties.ContainsKey("Email"))
                            {
                                Application.Current.Properties.Remove("Email");
                                Application.Current.Properties.Remove("Expires");
                                Application.Current.Properties.Remove("Id");

                                Application.Current.Properties.Add("Email", tryGetExistingAccount.Email);
                                Application.Current.Properties.Add("Expires", tryGetExistingAccount.TokenInfo.DateTime);
                                Application.Current.Properties.Add("Id", tryGetExistingAccount.Number.ToString());
                            }
                            else
                            {
                                Application.Current.Properties.Add("Email", tryGetExistingAccount.Email);
                                Application.Current.Properties.Add("Expires", tryGetExistingAccount.TokenInfo.DateTime);
                                Application.Current.Properties.Add("Id", tryGetExistingAccount.Number.ToString());
                            }

                            await App.Current.SavePropertiesAsync();
                            #endregion
                        }

                        #endregion

                        App.Gmail = Application.Current.Properties["Email"] as string;
                        App.ExpiredTime = Convert.ToDateTime(Application.Current.Properties["Expires"]);
                        App.Id = Application.Current.Properties["Id"] as string;

                        menu.spanGmail.Text = App.Gmail;
                        menu.spId.Text = App.Id.ToString();

                        var bases = db.Accounts.Include(us => us.UsersDatabases).ThenInclude(data => data.DbMenuListModel).Where(a => a.Email == App.Gmail).FirstOrDefault();
                        if (bases != null)
                        {
                            menu.field2.Children.Remove(menu.emptyDBView2);
                            menu.field2.Children.Add(menu.field3);
                            menu.scr1.Content = menu.field2;

                            foreach (var item in bases.UsersDatabases)
                            {
                                item.DbMenuListModel.IsLoading = "true";
                                menu.menuPageViewModel.DbList.Add(item.DbMenuListModel);
                            }
                        }
                        else
                        {
                            menu.scr1.Content = menu.field2;
                            menu.Content = menu.scr1;
                        }
                    }

                    menu.btnGo.IsVisible = true;
                    menu.hello.Text = "Привет!";
                    menu.youNotLogin.Text = "Здесь\nты\nнайдешь\nсвои\nбазы\nданных,\nно сначала\nнужно\nвойти";
                }
                else
                {
                    menu.btnGo.IsVisible = true;
                    menu.hello.Text = "Привет!";
                    menu.youNotLogin.Text = "Здесь\nты\nнайдешь\nсвои\nбазы\nданных,\nно сначала\nнужно\nвойти";

                    await menu.DisplayAlert("Упс!", "Не удалось войти в аккаунт", "ОК");
                }
            }
            else if (UriAuthData != null && prop == "UriAuthData" && UriAuthData.Contains("code=") == false)
            {
                MainPage current = App.Current.MainPage as MainPage;
                await current.DisplayAlert("Упс!", "Вы не вошли в аккаунт", "ОК");
            }
            else
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

        private async Task<GooglePersonModel> GetGoogleInfo(string token)
        {
            if (Device.RuntimePlatform == Device.UWP)
            {
                HttpClient client = new HttpClient();

                string request = "https://people.googleapis.com/v1/people/me?personFields=emailAddresses" 
                    + "&access_token=" + token;

                HttpResponseMessage response = await client.GetAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                GooglePersonModel profile = JsonConvert.DeserializeObject<GooglePersonModel>(json);
                return profile;
            }
            else
            {
                HttpClient client = new HttpClient();

                string request = "https://people.googleapis.com/v1/people/me?personFields=emailAddresses" +
                    "&access_token=" + token;

                HttpResponseMessage response = await client.GetAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                GooglePersonModel profile = JsonConvert.DeserializeObject<GooglePersonModel>(json);
                return profile;
            }

        }
    }
}
