using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MvcLib.Db {
    public class DbFactory : IDesignTimeDbContextFactory<AppDbContext> {
        public AppDbContext CreateDbContext (string[] args) {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext> ();        
            optionsBuilder.UseSqlite ("Data Source=pdf_testpaper.db");

            return new AppDbContext (optionsBuilder.Options);
        }
    }

}