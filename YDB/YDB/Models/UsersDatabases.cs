using System;
using System.Collections.Generic;
using System.Text;

namespace YDB.Models
{
    public class UsersDatabases
    {
        public string DbAccountModelEmail { get; set; }
        public DbAccountModel DbAccountModel { get; set; }

        public int DbMenuListModelId { get; set; }
        public DbMenuListModel DbMenuListModel { get; set; }
    }
}
