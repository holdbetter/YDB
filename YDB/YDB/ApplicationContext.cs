using System;
using System.Collections.Generic;
using System.Text;
using YDB.Models;
using Microsoft.EntityFrameworkCore;

namespace YDB
{
    public class ApplicationContext : DbContext
    {
        private string _databasePath;

        public DbSet<DbAccountModel> Accounts { get; set; }

        public ApplicationContext(string databasePath)
        {
            _databasePath = databasePath;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={_databasePath}");
            //optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=helloappdb;Trusted_Connection=True;");
        }
    }
}
