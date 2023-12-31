using Albums.Api.Data;
using Microsoft.Extensions.Options;

namespace Albums.Api.Configuration;
public static class DependencyInjectionConfig
{
    private static DbSettings? _dbSettings = new();
    public static IServiceCollection ResolveDependencies(this WebApplicationBuilder builder)
    {
        ConfigureDatabase(builder);
        InjectContexts(builder);
        InjectRepositories(builder);
        ConfigureSwagger(builder);
        
        return builder.Services;
    }

    private static void ConfigureDatabase(WebApplicationBuilder builder)
    {
        builder.Services.Configure<DbSettings>(builder.Configuration.GetSection(nameof(DbSettings)));

        var settings = builder.Configuration.GetSection("DbSettings").Get<DbSettings>();

        _dbSettings = settings;

        builder.Services.AddSingleton<AppDbContext>(serviceProvider =>
        {
            return new AppDbContext(settings.ConnectionString, settings.DatabaseName);
        });
    }

    private static void ConfigureSwagger(WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
    }

    private static void InjectContexts(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IAppDbContext>(_ =>   new AppDbContext(_dbSettings.ConnectionString, _dbSettings.DatabaseName));
    }

    private static void InjectRepositories(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IAlbumRepository, AlbumRepository>();
    }
}
