using System;
using YDB.Services;
using YDB.iOS;
using Xamarin.Forms;
using System.IO;

[assembly: Dependency(typeof(IosDbPath))]
namespace YDB.iOS
{
    class IosDbPath : IPathDatabase
    {
        public string GetDataBasePath(string sqliteFilename)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "..", "Library", sqliteFilename);
        }
    }
}