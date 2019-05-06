﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace YDB.Models
{
    [Table("GoogleInfo")]
    public class DbAccountModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string GoogleNumbers { get; set; }
        public TokenModel TokenInfo { get; set; }
    }
}
