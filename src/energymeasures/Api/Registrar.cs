namespace energymeasures.Api;

public static class Registrar
{
    public static void SetupApiDocumentation(this WebApplicationBuilder builder)
    {

        builder.Services.AddSwaggerDocument();
        builder.Services.AddEndpointsApiExplorer();
    }

    public static void SetupApiDocumentation(this IApplicationBuilder app)
    {
        app.UseOpenApi();
        app.UseSwaggerUi3();
    }
}