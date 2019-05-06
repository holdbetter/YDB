using System;
using System.Collections.Generic;
using System.Text;
using YDB.Views;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Xamarin.Forms;

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
}
