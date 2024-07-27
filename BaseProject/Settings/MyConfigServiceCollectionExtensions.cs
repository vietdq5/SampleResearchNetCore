using BaseProject.DataContext;
using Microsoft.EntityFrameworkCore;

namespace BaseProject.Settings;

public static class MyConfigServiceCollectionExtensions
{
    public static IServiceCollection AddConfig(
            this IServiceCollection services, IConfiguration config)
    {
        services.Configure<PostgreOptions>(config.GetSection(PostgreOptions.PostgreOption));

        return services;
    }

    public static IServiceCollection AddMyDBContext(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddDbContext<ProjectDbContext>(option => option.UseNpgsql(config[$"{PostgreOptions.PostgreOption}:{nameof(PostgreOptions.LocalConnectionString)}"]));
        return services;
    }

    public static IServiceCollection AddMyDependencyGroup(
         this IServiceCollection services)
    {
        //services.AddScoped<IMyDependency, MyDependency>();

        return services;
    }
}