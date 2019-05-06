using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace YDB.Models
{
    [DataContract]
    class GoogleProfileModel
    {
        [DataMember]
        public Name Name;
        [DataMember]
        public Emails[] Emails;
        [DataMember]
        public string Id;
    }

    class Name
    {
        public string GivenName;
        string FamilyName;
    }

    class Emails
    {
        public string Value;
        string Type;
    }
}
