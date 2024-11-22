using Microsoft.EntityFrameworkCore;
using TestWebApp.Model.Types;

namespace TestWebApp.Model
{
    public sealed class AppDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
    }
}
