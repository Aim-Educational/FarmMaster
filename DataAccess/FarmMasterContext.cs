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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            base.OnModelCreating(modelBuilder);
        }

        #region Tables
        public DbSet<Settings> Settings { get; set; }
        #endregion
    }

    public class TrueFarmMasterContextFactory : FarmMasterContextFactory<FarmMasterContext>
    {
    }
}
