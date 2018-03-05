using CoAPExplorer.Models;
using CoAPExplorer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoAPExplorer.Database
{
    public class CoapExplorerContextFactory : IDesignTimeDbContextFactory<CoapExplorerContext>
    {
        public CoapExplorerContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CoapExplorerContext>();
            optionsBuilder.UseSqlite("Filename=something.db", b => b.MigrationsAssembly("Database"));

            return new CoapExplorerContext(optionsBuilder.Options);
        }
    }
}
