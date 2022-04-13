using Lab4N.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab4N.Data
{
    public class MarketDbContext : DbContext
    {
        public MarketDbContext(DbContextOptions<MarketDbContext> options) : base(options)
        {
        }
        public DbSet<Client> Clients { get; set; }//
        public DbSet<Brokerage> Brokerages { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<AdsBrokerage> AdvertisementBrokerage { get; set; }
        public DbSet<Advertisement> Advertisements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>().ToTable("Client");
            modelBuilder.Entity<Brokerage>().ToTable("Brokerage");
            modelBuilder.Entity<AdsBrokerage>().ToTable("AdvertisementBrokerage");
            modelBuilder.Entity<Advertisement>().ToTable("Advertisement");
            modelBuilder.Entity<Subscription>()
                .HasKey(c => new { c.ClientId, c.BrokerageId });

        }
    }
}
