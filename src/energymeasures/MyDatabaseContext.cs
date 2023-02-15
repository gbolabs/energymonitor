using Microsoft.EntityFrameworkCore;

namespace energymeasures;

internal class MyDatabaseContext : DbContext
{
    public MyDatabaseContext(DbContextOptions<MyDatabaseContext> options)
        : base(options)
    {
    }

    public DbSet<Sensor>? Sensors { get; set; }
    public DbSet<CounterDefinition>? CounterDefinitions { get; set; }
    public DbSet<LastMeasure>? LastMeasures { get; set; }
    public DbSet<Measure>? Measures { get; set; }
}