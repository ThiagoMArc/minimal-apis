using Albums.Api.Data;

namespace Albums.Api.Configuration;
public static class DependencyInjectionConfig
{
    private static string connectionString = "";
    
    public static IServiceCollection ResolveDependencies(this WebApplicationBuilder builder)
    {
        ConfigureDatabase(builder);
        InjectContext(builder);
        InjectRepositories(builder);
        AddDatabaseDeveloperPageExceptionFilter(builder);
        ConfigureSwagger(builder);

        return builder.Services;
    }

    private static void ConfigureDatabase(WebApplicationBuilder builder)
    {
        connectionString = builder.Configuration["DbContextSettings:ConnectionString"] ?? "";

        builder.Services.AddSingleton<AppDbContext>(serviceProvider =>
        {
            return new AppDbContext(connectionString);
        });
    }

    private static void InjectContext(WebApplicationBuilder builder)
    {
         builder.Services.AddSingleton<IAppDbContext>(_ =>   new AppDbContext(connectionString));
    }

    private static void InjectRepositories(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IAlbumRepository, AlbumRepository>();
    }

    private static void AddDatabaseDeveloperPageExceptionFilter(WebApplicationBuilder builder)
    {
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
    }

    private static void ConfigureSwagger(WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
    }
}
