using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.Web.Http;
using Xamarin.Forms;
using YDB.Models;
using YDB.Services;
using YDB.UWP;

[assembly: Dependency(typeof(UwpLoginRequest))]
namespace YDB.UWP
{
    class UwpLoginRequest : IUwpLoginRequest
    {
        public async void StartRequestAndGetResults()
        {
            string googleUrl = "https://accounts.google.com/o/oauth2/v2/auth" + "?client_id=" + YDB.App.ClientIdUWP +
                                        "&redirect_uri=" + "https://google.com" +
                                        "&response_type=code" +
                                        "&scope=" + YDB.App.Scope;

            Uri startUri = new Uri(googleUrl);
            Uri endUri = new Uri("https://google.com");

            WebAuthenticationResult result = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, startUri, endUri);

            string code = result.ResponseStatus != WebAuthenticationStatus.Success ? null : result.ResponseData;

            if (code != null)
            {
                (YDB.App.Current.MainPage as YDB.Views.MainPage).authentication.UriAuthData = code;
            }
        }
    }
}
