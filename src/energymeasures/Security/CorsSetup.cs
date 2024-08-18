namespace energymeasures.Security;

public static class CorsSetup
{
    internal static void SetupCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("Production", policyBuilder =>
            {
                policyBuilder.WithOrigins("https://energy.isago.ch", // custom domain
                    "https://salmon-coast-0abc20703.2.azurestaticapps.net/", // direct link
                    "http://localhost:8888", // local dev (docker)
                    "https://salmon-coast-0abc20703-preview.westeurope.2.azurestaticapps.net/"); // preview
                policyBuilder.AllowAnyHeader();
                policyBuilder.WithMethods("GET", "POST");
            });
            options.AddPolicy(name: "Development",
                builder =>
                {
                    builder.WithMethods("GET", "POST");
                    builder.WithOrigins("http://localhost:4200");
                    builder.WithOrigins("http://localhost:5000"); // For redirects
                    builder.AllowAnyHeader();
                });
            options.DefaultPolicyName = builder.Environment.IsDevelopment() ? "Development" : "Production";
        });
    }

    internal static void SetupCors(this IApplicationBuilder app)
    {
        app.UseCors();
    }
}
