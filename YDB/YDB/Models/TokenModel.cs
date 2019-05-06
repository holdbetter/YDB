using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace YDB.Models
{
    [DataContract]
    public class TokenModel
    {
        [DataMember]
        [Key]
        public string Access_token { get; set; }
        [DataMember]
        public string Id_token { get; set; }
        [DataMember]
        public int Expires_in { get; set; }
        [DataMember]
        public string Token_type { get; set; }
        [DataMember]
        public string Scope { get; set; }
        [DataMember]
        public string Refresh_Token { get; set; }
    }
}
