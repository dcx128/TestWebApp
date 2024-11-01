using TestWebApp.Model;
using TestWebApp.Services;
using TestWebApp.Settings;

namespace TestWebApp
{
    public static class StartupExtensions
    {
        public static WebApplicationBuilder Configure(this WebApplicationBuilder builder)
        {
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            builder.Host.ConfigureServices((context, services) =>
            {
                services.AddSingleton<IProductSettings>(_ =>
                {
                    var productSettings = new ProductSettings();
                    context.Configuration.Bind(productSettings);
                    return productSettings;
                });

                services.AddSingleton<IHashService, HashService>();

                services.AddDbContextFactory<AppDbContext>(lifetime: ServiceLifetime.Singleton);
            });

            return builder;
        }
    }
}
