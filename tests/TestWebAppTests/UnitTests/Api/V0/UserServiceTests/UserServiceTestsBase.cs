using Microsoft.EntityFrameworkCore;
using TestWebApp.Api.V0;
using TestWebApp.Model;
using TestWebApp.Model.Types;
using TestWebApp.Services;
using TestWebAppTests.UnitTests.Base;
using TestWebAppTests.UnitTests.MotherFactories;

namespace TestWebAppTests.UnitTests.Api.V0.UserServiceTests
{
    public abstract class UserServiceTestsBase : TestBase
    {
        protected const string testUserName = nameof(testUserName);
        protected const string testUserPass = nameof(testUserPass);
        protected const string testSession = nameof(testSession);

        protected readonly TimeSpan callTimeout = TimeSpan.FromMicroseconds(250);
        protected readonly IEnumerable<User> testUser =
            [new User { UserName = testUserName, PassHash = testUserPass, Session = testSession }];

        protected UserService CreateSut(
            IDbContextFactory<AppDbContext>? contextFactory = null,
            ILdapService? ldapService = null)
        {
            return mf.UserService(contextFactory: contextFactory, ldapService: ldapService);
        }

        [SetUp]
        public async Task SetUp()
        {
            using var context = await mf.DbContextFactory()
                .WithNoUsers()
                .CreateDbContextAsync(default);
            context.Database.EnsureDeleted(); // Cleaning up DB initialized in memory
            await context.SaveChangesAsync();
        }
    }
}
