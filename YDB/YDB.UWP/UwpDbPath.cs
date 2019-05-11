using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YDB.Services;
using YDB.UWP;
using System.IO;
using Xamarin.Forms;
using Windows.Storage;
using Windows.Security.Authentication.Web;

[assembly: Dependency(typeof(UwpDbPath))]
namespace YDB.UWP
{
    class UwpDbPath : IPathDatabase
    {
        public string GetDataBasePath(string sqliteFilename)
        {
            //System.Diagnostics.Debug.WriteLine(Path.Combine(ApplicationData.Current.LocalFolder.Path, sqliteFilename));
            return Path.Combine(ApplicationData.Current.LocalFolder.Path, sqliteFilename);
        }
    }
}
