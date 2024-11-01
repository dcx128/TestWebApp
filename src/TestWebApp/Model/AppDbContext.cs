using Microsoft.EntityFrameworkCore;
using Npgsql;
using TestWebApp.Model.Types;
using TestWebApp.Settings;

namespace TestWebApp.Model
{
    public class AppDbContext(IProductSettings productSettings) : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(productSettings.DbConnectionString);
            optionsBuilder.UseNpgsql(dataSourceBuilder.Build());
        }
    }
}
