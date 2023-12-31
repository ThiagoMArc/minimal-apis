using Albums.Api.Data;

namespace Albums.Api.Configuration;
public static class DependencyInjectionConfig
{
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
        builder.Services.AddDbContext<AppDbContext>();
    }

    private static void InjectContext(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IAppDbContext, AppDbContext>();
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
