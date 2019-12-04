using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace YDB.Models
{
    [DataContract]
    public class TokenModel : INotifyPropertyChanged
    {
        private int expires_in;
        private DateTime dateTime;

        [DataMember]
        [Key]
        public string Access_token { get; set; }
        [DataMember]
        public string Id_token { get; set; }
        [DataMember]
        public int Expires_in
        {
            get
            {
                return expires_in;
            }
            set
            {
                expires_in = value;
                dateTime = (DateTime.UtcNow).AddHours(1);
            }
        }
        [DataMember]
        public string Token_type { get; set; }
        [DataMember]
        public string Scope { get; set; }

        public DateTime DateTime
        {
            get
            {
                return dateTime;
            }
            set
            {
                dateTime = value;
                OnPropertyChanged("DateTime");
            }
        }

        public string DbAccountModelEmail { get; set; }
        public DbAccountModel DbAccountModel { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
