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
        public DbSet<DbMenuListModel> DatabasesList { get; set; }

        public ApplicationContext(string databasePath)
        {
            _databasePath = databasePath;
            this.Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UsersDatabases>()
                .HasKey(t => new { t.DbAccountModelId, t.DbMenuListModelId });

            modelBuilder.Entity<UsersDatabases>()
                .HasOne(pt => pt.DbAccountModel)
                .WithMany(p => p.UsersDatabases)
                .HasForeignKey(pt => pt.DbAccountModelId);

            modelBuilder.Entity<UsersDatabases>()
                .HasOne(pt => pt.DbMenuListModel)
                .WithMany(t => t.UsersDatabases)
                .HasForeignKey(pt => pt.DbMenuListModelId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={_databasePath}");
            //optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=helloappdb;Trusted_Connection=True;");
        }
    }
}
