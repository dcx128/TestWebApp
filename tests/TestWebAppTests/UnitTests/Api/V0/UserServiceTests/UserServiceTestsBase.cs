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
        protected const string testUserName = "test";
        protected const string testUserPass = "test";
        protected const string testSession = "test";

        protected readonly TimeSpan callTimeout = TimeSpan.FromMicroseconds(250);
        protected readonly IEnumerable<User> noUsers = [];
        protected readonly IEnumerable<User> testUser = [new User { UserName = testUserName, PassHash = testUserPass, Session = testSession }];

        protected UserService CreateSut(IDbContextFactory<AppDbContext>? contextFactory = null, ILdapService? ldapService = null) =>
            mf.UserService(contextFactory: contextFactory, ldapService: ldapService);
    }
}
