using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace YDB.Models
{
    [DataContract]
    public class TokenModel
    {
        [DataMember]
        public string Access_token { get; set; }
        [DataMember]
        string Id_token { get; set; }
        [DataMember]
        int Expires_in { get; set; }
        [DataMember]
        string Token_type { get; set; }
        [DataMember]
        string Scope { get; set; }
        [DataMember]
        string Refresh_Token { get; set; }
    }
}
