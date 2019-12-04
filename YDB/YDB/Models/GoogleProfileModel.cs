using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace YDB.Models
{
    [DataContract]
    class GooglePersonModel
    {
        [DataMember]
        public string ResourceName;
        [DataMember]
        public EmailAddresses[] EmailAddresses;
        [DataMember]
        public string Id;
    }

    class EmailAddresses
    {
        public string Value;
        string Type;
    }
}
