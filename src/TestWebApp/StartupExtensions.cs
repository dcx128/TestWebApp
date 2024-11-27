using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Npgsql;
using TestWebApp.Model;
using TestWebApp.Services;
using TestWebApp.Settings;

namespace TestWebApp
{
    public static class StartupExtensions
    {
        public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder, params string[] args)
        {
            builder.Configuration
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddCommandLine(args)
                .AddEnvironmentVariables();

            builder.Host.ConfigureServices((context, services) =>
            {
                services.AddSingleton<IProductSettings>(provider =>
                {
                    ChangeToken.OnChange(
                        () => context.Configuration.GetReloadToken(),
                        () => context.Configuration.Bind(provider.GetRequiredService<IProductSettings>()));

                    var productSettings = new ProductSettings();
                    context.Configuration.Bind(productSettings);
                    return productSettings;
                });

                services.AddSingleton<IHashService, HashService>();
                services.AddSingleton<IHashGenerator, HashGenerator>();
                services.AddSingleton<ILdapService, LdapService>();

                services.AddDbContextFactory<AppDbContext>(
                    lifetime: ServiceLifetime.Singleton,
                    optionsAction: (provider, optionsBuilder) =>
                    {
                        var productSettings = provider.GetRequiredService<IProductSettings>();
                        var dataSourceBuilder = new NpgsqlDataSourceBuilder(productSettings.DbConnectionString);
                        optionsBuilder.UseNpgsql(dataSourceBuilder.Build());
                    });
            });

            builder.Services
                .AddEndpointsApiExplorer() // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                .AddSwaggerGen()
                .AddControllers();

            return builder;
        }

        public static T If<T>(this T obj, bool condition, Action<T> then)
        {
            if (condition)
            {
                then(obj);
            }

            return obj;
        }
    }
}
