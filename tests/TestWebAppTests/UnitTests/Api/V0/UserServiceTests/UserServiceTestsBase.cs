using Microsoft.EntityFrameworkCore;
using TestWebApp.Api.V0;
using TestWebApp.Model;
using TestWebApp.Services;
using TestWebAppTests.UnitTests.Base;
using TestWebAppTests.UnitTests.MotherFactories;

namespace TestWebAppTests.UnitTests.Api.V0.UserServiceTests
{
    public abstract class UserServiceTestsBase : TestBase
    {
        protected const string testUserName = "test";
        protected const string testUserPass = "test";

        protected readonly TimeSpan callTimeout = TimeSpan.FromMicroseconds(250);

        protected UserService CreateSut(IDbContextFactory<AppDbContext>? contextFactory = null, ILdapService? ldapService = null) =>
            mf.UserService(contextFactory: contextFactory, ldapService: ldapService);
    }
}
