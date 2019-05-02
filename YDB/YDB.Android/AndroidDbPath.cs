using System;
using YDB.Droid;
using YDB.Services;
using System.IO;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidDbPath))]
namespace YDB.Droid
{
    public class AndroidDbPath : IPathDatabase
    {
        public string GetDataBasePath(string filename)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), filename);
        }
    }
}