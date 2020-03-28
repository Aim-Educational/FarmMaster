using DataAccess.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess
{
    public class FarmMasterContext : DbContext
    {
        public FarmMasterContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);
        }

        #region Tables
        public DbSet<Settings> Settings   { get; set; }
        public DbSet<LogEntry> LogEntries { get; set; }
        public DbSet<Contact>  Contacts   { get; set; }
        #endregion
    }

    public class TrueFarmMasterContextFactory : FarmMasterContextFactory<FarmMasterContext>
    {
    }
}
