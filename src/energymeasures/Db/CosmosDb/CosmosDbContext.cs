using Microsoft.EntityFrameworkCore;

namespace energymeasures;

internal class CosmosDbContext : DbContext
{
    public CosmosDbContext(DbContextOptions<CosmosDbContext> dbContextOptions) : base(dbContextOptions)
    {
    }

    public DbSet<PowerMeasureRead> PowerMeasures { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PowerMeasureRead>()
            .HasNoDiscriminator()
            .ToContainer("RawMeasures")
            .HasPartitionKey(pm => pm.samplingdate)
            .HasKey(pm => pm.Id);
    }
}