using Albums.Api.Data;
using Albums.Api.ViewModels;
using Albums.Api.ViewModels.Album;
using FluentValidation;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

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
        RegisterValidators(builder);
        AddFluentAutoValidation(builder);

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

    public static void RegisterValidators(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IValidator<AlbumViewModel>, AlbumViewModelValidator>();
        builder.Services.AddScoped<IValidator<AlbumUpdateViewModel>, AlbumUpdateViewModelValidator>();
    }

    public static void AddFluentAutoValidation(WebApplicationBuilder builder)
    {
        builder.Services.AddFluentValidationAutoValidation();
    }
}
