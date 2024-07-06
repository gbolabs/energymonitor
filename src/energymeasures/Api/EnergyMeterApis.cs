using common;
using energymeasures.Helpers;
using Microsoft.EntityFrameworkCore;

namespace energymeasures.Api;

public static class EnergyMeterApis
{
    public static void RegisterEnergyMeterApis(this WebApplication app)
    {
        app.MapPost("/api/energymeter",
            async (CosmosDbContext dbContext, PowerMeasure measure, CancellationToken cancellationToken) =>
            {
                var entity = await dbContext.PowerMeasures.AddAsync(measure.ToCosmosPowerMeasure(), cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);
                return Results.Created($"/api/energymeter/{entity.Entity.Id}", measure);
            });

        app.MapGet("/api/energymeter/{id}", async (CosmosDbContext dbContext, string id) =>
        {
            var measure = await dbContext.PowerMeasures.FindAsync(id);
            if (measure == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(measure);
        });

        app.MapGet("/api/energymeter", async (CosmosDbContext dbContext) =>
        {
            var measures = await dbContext.PowerMeasures.ToListAsync();
            return Results.Ok(measures);
        });

        app.MapDelete("/api/energymeter/{id}", async (CosmosDbContext dbContext, string id) =>
        {
            var measure = await dbContext.PowerMeasures.FindAsync(id);
            if (measure == null)
            {
                return Results.NotFound();
            }

            dbContext.PowerMeasures.Remove(measure);
            await dbContext.SaveChangesAsync();
            return Results.NoContent();
        });
    }
}
