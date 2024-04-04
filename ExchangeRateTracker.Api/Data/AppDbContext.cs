using ExchangeRateTracker.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ExchangeRateTracker.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ExchangeRate> ExchangeRates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<ExchangeRate>()
                .HasKey(model => new { model.CurrencyCode, model.Date });
        }
    }
}
