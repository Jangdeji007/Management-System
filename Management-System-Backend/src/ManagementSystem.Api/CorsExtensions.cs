namespace ManagementSystem.Api;

public static class CorsExtensions
{
    public const string FrontendPolicyName = "FrontendCors";

    private static readonly string[] DevelopmentDefaultOrigins =
    [
        "http://localhost:5173",
        "http://localhost:5034"
    ];

    public static IServiceCollection AddFrontendCors(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var origins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
        if (origins is null or { Length: 0 } && environment.IsDevelopment())
            origins = DevelopmentDefaultOrigins;

        services.AddCors(options =>
        {
            options.AddPolicy(FrontendPolicyName, policy =>
            {
                if (origins is { Length: > 0 })
                    policy.WithOrigins(origins);
                else
                    policy.SetIsOriginAllowed(_ => false);

                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
            });
        });

        return services;
    }
}
