using FishReportApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FishReportApi.Data
{
  public class FishDBContext : DbContext
  {
    public FishDBContext(DbContextOptions<FishDBContext> options) : base(options)
    {
    }

    public DbSet<Species> Species { get; set; }
    public DbSet<FishMarket> FishMarkets { get; set; }
    public DbSet<FishMarketInventory> FishMarketInventory { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<Species>()
     .Property(s => s.Price)
     .HasPrecision(18, 4);

      modelBuilder.Entity<FishMarketInventory>()
                   .HasKey(fmi => new { fmi.FishMarketId, fmi.SpeciesId });

      modelBuilder.Entity<FishMarketInventory>()
          .HasOne(fmi => fmi.FishMarket)
          .WithMany(fm => fm.FishMarketInventory)
          .HasForeignKey(fmi => fmi.FishMarketId);

      modelBuilder.Entity<FishMarketInventory>()
          .HasOne(fmi => fmi.Species)
          .WithMany(s => s.FishMarketInventory)
          .HasForeignKey(fmi => fmi.SpeciesId);


      // Seed Fish Markets
      modelBuilder.Entity<FishMarket>().HasData(
         new FishMarket { Id = 1, MarketName = "NY Fish Co.", Location = "New York, NY" },
         new FishMarket { Id = 2, MarketName = "Tempest", Location = "New Bedford, MA" },
         new FishMarket { Id = 3, MarketName = "Lobster House", Location = "Cape May, NJ" }
     );

      // Seed Fish Species
      modelBuilder.Entity<Species>().HasData(
        new Species { Id = 1, Name = "Bluefin Tuna", Habitat = "Open Ocean", Length = 300, Population = 25000, Lifespan = 15, Price = 12.99M, },
        new Species { Id = 2, Name = "Mahi Mahi", Habitat = "Tropical Waters", Length = 180, Population = 75000, Lifespan = 5, Price = 8.50M, },
        new Species { Id = 3, Name = "Swordfish", Habitat = "Deep Sea", Length = 400, Population = 30000, Lifespan = 9, Price = 14.25M },
        new Species { Id = 4, Name = "Atlantic Salmon", Habitat = "Rivers and Oceans", Length = 150, Population = 40000, Lifespan = 8, Price = 9.75M },
        new Species { Id = 5, Name = "Yellowfin Tuna", Habitat = "Tropical Oceans", Length = 250, Population = 60000, Lifespan = 7, Price = 11.20M },
        new Species { Id = 6, Name = "Humphead Wrasse", Habitat = "Coral Reefs", Length = 230, Population = 12000, Lifespan = 30, Price = 18.00M },
        new Species { Id = 7, Name = "Anchovy", Habitat = "Coastal Waters", Length = 15, Population = 1000000, Lifespan = 4, Price = 2.00M },
        new Species { Id = 8, Name = "Marlin", Habitat = "Open Ocean", Length = 420, Population = 20000, Lifespan = 12, Price = 13.80M },
        new Species { Id = 9, Name = "Pacific Halibut", Habitat = "Northern Pacific", Length = 240, Population = 35000, Lifespan = 25, Price = 10.50M },
        new Species { Id = 10, Name = "Giant Trevally", Habitat = "Coral and Rocky Reefs", Length = 170, Population = 18000, Lifespan = 10, Price = 9.90M },
        new Species { Id = 11, Name = "King Mackerel", Habitat = "Warm Coastal Waters", Length = 180, Population = 50000, Lifespan = 7, Price = 7.99M },
        new Species { Id = 12, Name = "Tarpon", Habitat = "Coastal and Estuarine Waters", Length = 200, Population = 22000, Lifespan = 50, Price = 6.49M },
        new Species { Id = 13, Name = "Sailfish", Habitat = "Tropical Oceans", Length = 330, Population = 27000, Lifespan = 4, Price = 15.25M },
        new Species { Id = 14, Name = "Snapper", Habitat = "Reefs", Length = 80, Population = 62000, Lifespan = 15, Price = 6.99M },
        new Species { Id = 15, Name = "Groupers", Habitat = "Reefs and Rocky Areas", Length = 250, Population = 40000, Lifespan = 20, Price = 10.25M },
        new Species { Id = 16, Name = "Flying Fish", Habitat = "Warm Oceans", Length = 30, Population = 85000, Lifespan = 5, Price = 5.50M },
        new Species { Id = 17, Name = "Pompano", Habitat = "Shallow Coastal Waters", Length = 60, Population = 30000, Lifespan = 7, Price = 8.25M },
        new Species { Id = 18, Name = "Sturgeon", Habitat = "Rivers and Coastal Waters", Length = 350, Population = 15000, Lifespan = 60, Price = 16.50M }
          );

      modelBuilder.Entity<FishMarketInventory>().HasData(
   new FishMarketInventory { FishMarketId = 1, SpeciesId = 1 },
new FishMarketInventory { FishMarketId = 1, SpeciesId = 2 },
new FishMarketInventory { FishMarketId = 1, SpeciesId = 4 },
new FishMarketInventory { FishMarketId = 1, SpeciesId = 6 },
new FishMarketInventory { FishMarketId = 1, SpeciesId = 7 },
new FishMarketInventory { FishMarketId = 1, SpeciesId = 9 },
new FishMarketInventory { FishMarketId = 1, SpeciesId = 10 },

new FishMarketInventory { FishMarketId = 2, SpeciesId = 3 },
new FishMarketInventory { FishMarketId = 2, SpeciesId = 5 },
new FishMarketInventory { FishMarketId = 2, SpeciesId = 8 },
new FishMarketInventory { FishMarketId = 2, SpeciesId = 11 },
new FishMarketInventory { FishMarketId = 2, SpeciesId = 12 },
new FishMarketInventory { FishMarketId = 2, SpeciesId = 14 },
new FishMarketInventory { FishMarketId = 2, SpeciesId = 16 },
new FishMarketInventory { FishMarketId = 2, SpeciesId = 18 },

new FishMarketInventory { FishMarketId = 3, SpeciesId = 4 },
new FishMarketInventory { FishMarketId = 3, SpeciesId = 7 },
new FishMarketInventory { FishMarketId = 3, SpeciesId = 9 },
new FishMarketInventory { FishMarketId = 3, SpeciesId = 13 },
new FishMarketInventory { FishMarketId = 3, SpeciesId = 15 },
new FishMarketInventory { FishMarketId = 3, SpeciesId = 17 }
      );
    }
  }
}