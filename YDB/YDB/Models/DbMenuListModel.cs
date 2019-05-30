using System;
using System.Collections.Generic;
using System.Text;
using YDB.Views;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Xamarin.Forms;
using System.ComponentModel.DataAnnotations;

namespace YDB.Models
{
    [Table("DatabaseInfo")]
    public class DbMenuListModel : INotifyPropertyChanged
    {
        private MarkerCustomView marker;
        private List<int> invitedUsers;

        public int Id { get; set; }
        public string Name { get; set; }
        public string HexColor { get; set; }
        public bool IsPrivate { get; set; }
        public string Carrier { get; set; } //владелец
        public DatabaseData DatabaseData { get; set; }
        public List<UsersDatabases> UsersDatabases { get; set; }

        public DbMenuListModel()
        {
            DatabaseData = new DatabaseData();
            UsersDatabases = new List<UsersDatabases>();
        }

        [NotMapped]
        public string IsLoading { get; set;}

        [NotMapped]
        public int IsLoadingId { get; set; }

        [NotMapped]
        public List<int> InvitedUsers
        {
            get
            {
                return invitedUsers;
            }
            set
            {
                if (IsPrivate)
                {
                    invitedUsers = value;
                }
            }
        }

        [NotMapped]
        public MarkerCustomView Marker
        {
            get
            {
                return marker;
            }
            set
            {
                marker = value;
                HexColor = value.HexColor;
                MarkerColor = Color.FromHex(HexColor);
                OnPropertyChanged("Marker");
            }
        }

        [NotMapped]
        public Color MarkerColor { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }

    public class InvitedUsers
    {
        public int Id { get; set; }
        public int UserNumber { get; set; }

        public string DbMenuListModelName { get; set; }
        public DbMenuListModel DbMenuListModel { get; set; }
    }

    public class DatabaseData
    {
        public int Id { get; set; }
        public string DatabaseName { get; set; }
        
        public DatabaseData()
        {
            Data = new List<KeysAndTypes>();
        }

        public int DbMenuListModelId { get; set; }
        public DbMenuListModel DbMenuListModel { get; set; }

        public List<KeysAndTypes> Data { get; set; }
    }

    public class KeysAndTypes
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Type { get; set; }

        public int DatabaseDataId { get; set; }
        public DatabaseData DatabaseData { get; set; }

        public KeysAndTypes(string key, string type)
        {
            Key = key;
            Type = type;

            Values = new List<Values>();
        }

        public List<Values> Values { get; set; }
    }

    public class Values
    {
        public int Id { get; set; }
        public string Value { get; set; }

        public int KeysAndTypesId { get; set; }
        public KeysAndTypes KeysAndTypes { get; set; }
    }
}
