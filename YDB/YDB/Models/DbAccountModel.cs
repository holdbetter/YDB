using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace YDB.Models
{
    [Table("GoogleInfo")]
    public class DbAccountModel
    {
        private int number;

        [Key]
        public string Email { get; set; }
        public int Number { get; set; }
        public string GoogleNumbers { get; set; }
        public TokenModel TokenInfo { get; set; }

        public List<UsersDatabases> UsersDatabases { get; set; }

        public DbAccountModel()
        {
            UsersDatabases = new List<UsersDatabases>();
        }
    }
}
