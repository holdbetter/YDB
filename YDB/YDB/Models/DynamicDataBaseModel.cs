using System;
using System.Collections.Generic;
using System.Text;
using System.Dynamic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Xamarin.Forms;

namespace YDB.Models
{
    public class DynamicDataBaseModel : DynamicObject, INotifyPropertyChanged
    {
        [NotMapped]
        public string DatabaseName {
            get
            {
                return DatabaseName;
            }
            set
            {
                DatabaseName = value;
                OnPropertyChanged("DatabaseName");
            }
        }

        [NotMapped]
        public string DatabaseColor
        {
            get
            {
                return DatabaseColor;
            }
            set
            {
                DatabaseName = value;
                OnPropertyChanged("DatabaseColor");
                ColorChanged(value);
            }
        }

        [NotMapped]
        public Color MarkerColor { get; set; }

        public string Test2 { get; set; }
        public string Test3 { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public void ColorChanged(string color)
        {
            MarkerColor = Color.FromHex(color);
        }
    }
}
