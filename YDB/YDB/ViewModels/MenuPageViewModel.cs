using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Essentials;
using YDB.Models;
using YDB.Views;
using YDB.Services;
using System.Collections.Specialized;

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
            DbList.CollectionChanged += DbList_CollectionChanged;

            EnterInAppBtn = new Command(async() =>
            {
                if (Device.RuntimePlatform == Device.UWP)
                {
                    DependencyService.Get<IUwpLoginRequest>().StartRequestAndGetResults();
                }
                else
                {
                    string authRequest = App.AuthorizeUrl +
                    "?redirect_uri=" + App.RedirectUrl +
                    "&prompt=consent" +
                    "&response_type=code" +
                    "&client_id=" + App.ClientId +
                    "&scope=" + App.Scope;

                    await Browser.OpenAsync(authRequest, BrowserLaunchMode.SystemPreferred);
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
                        BarBackgroundColor = Color.FromHex("#d83434"),
                        BarTextColor = Color.White
                    };

                    if (Device.RuntimePlatform == Device.Android)
                    {
                        await Task.Delay(100);
                    }
                }
                
                current.IsPresented = false;
            });
        }

        private void DbList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedAction action = e.Action;

            var list = sender as ObservableCollection<DbMenuListModel>;

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var item = e.NewItems[0] as DbMenuListModel;

                if (item != null)
                {
                    MenuPage master = (App.Current.MainPage as MainPage).Master as MenuPage;
                    ListDbView listDbView = new ListDbView(item);
                    master.databaseListStack.Children.Insert(e.NewStartingIndex, listDbView);
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                MenuPage master = (App.Current.MainPage as MainPage).Master as MenuPage;
                master.databaseListStack.Children.RemoveAt(e.OldStartingIndex);
            }
        }
    }
}
