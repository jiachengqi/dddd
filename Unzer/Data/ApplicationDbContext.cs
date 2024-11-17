using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Unzer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Company> Companies { get; set; }
        public DbSet<Owner> Owners { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {          
            modelBuilder.Entity<Company>()
            .HasMany(c => c.Owners)
            .WithOne(o => o.Company)
            .HasForeignKey(o => o.CompanyId);

            base.OnModelCreating(modelBuilder);
        }
    }
}

