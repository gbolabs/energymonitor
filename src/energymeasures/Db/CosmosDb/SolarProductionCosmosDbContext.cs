using Microsoft.EntityFrameworkCore;

namespace energymeasures;

class SolarProductionCosmosDbContext : DbContext
{
    public SolarProductionCosmosDbContext(DbContextOptions<SolarProductionCosmosDbContext> dbContextOptions) : base(
        dbContextOptions)
    {
    }

    public DbSet<LastProduction> LastProductions { get; set; }
    public DbSet<DailyAverageProduction> Daily { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LastProduction>()
            .HasNoDiscriminator()
            .HasPartitionKey(e => e.PartitionKey)
            .ToContainer("last")
            .HasKey(e => e.Id);

        modelBuilder.Entity<DailyAverageProduction>()
            .HasNoDiscriminator()
            .HasPartitionKey(e => e.PartitionKey)
            .ToContainer("daily-average")
            .HasKey(e => e.Id);
    }
}