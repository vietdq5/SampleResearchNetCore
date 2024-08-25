using BaseProject.DataContext;
using BaseProject.Services;
using Elastic.Clients.Elasticsearch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BaseProject.Settings;

public static class MyConfigServiceCollectionExtensions
{
    public static IServiceCollection AddConfig(
            this IServiceCollection services, IConfiguration config)
    {
        services.Configure<PostgreOptions>(config.GetSection(PostgreOptions.PostgreOption));
        services.Configure<ElasticsearchOptions>(config.GetSection(ElasticsearchOptions.ElasticsearchOption));

        return services;
    }

    public static IServiceCollection AddMyDBContext(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddDbContext<ProjectDbContext>(option =>
        {
            option.UseNpgsql(config[$"{PostgreOptions.PostgreOption}:{nameof(PostgreOptions.LocalConnectionString)}"]);
        });

        return services;
    }

    public static IServiceCollection AddMyDependencyGroup(
         this IServiceCollection services)
    {
        // add ElasticSearch
        services.AddSingleton(sp =>
        {
            var elasticSettings = sp.GetRequiredService<IOptions<ElasticsearchOptions>>();
            var settings = new ElasticsearchClientSettings(new Uri(elasticSettings.Value.Uri))
                .DefaultIndex(elasticSettings.Value.DefaultIndex);
            return new ElasticsearchClient(settings);
        });
        services.AddScoped<IElasticsearchService, ElasticsearchService>();

        return services;
    }
}