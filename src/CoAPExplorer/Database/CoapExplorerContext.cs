using CoAPExplorer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoAPExplorer.Database
{
    public class CoapExplorerContext : DbContext
    {
        private readonly string _databasePath;

        public DbSet<Device> Devices { get; set; }

        public DbSet<Message> RecentMessages { get; set; }

        public string DatabasePath => _databasePath;

        public CoapExplorerContext(DbContextOptions<CoapExplorerContext> options)
            :base(options)
        { }

        public CoapExplorerContext(string databasePath)
        {
            _databasePath = databasePath;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!string.IsNullOrEmpty(_databasePath))
                optionsBuilder.UseSqlite($"Filename={_databasePath}");

            base.OnConfiguring(optionsBuilder);
        }
    }
}
