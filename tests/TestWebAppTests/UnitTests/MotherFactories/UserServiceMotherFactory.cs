using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TestWebApp.Api.V0;
using TestWebApp.Model;
using TestWebApp.Services;
using TestWebAppTests.UnitTests.Base;

namespace TestWebAppTests.UnitTests.MotherFactories
{
    public static class UserServiceMotherFactory
    {
        public static UserService UserService(this MotherFactory _,
            ILogger<UserService>? logger = null,
            IDbContextFactory<AppDbContext>? contextFactory = null,
            IHashGenerator? hashGenerator = null,
            ILdapService? ldapService = null)
        {
            logger ??= Mock.Of<ILogger<UserService>>();
            contextFactory ??= Mock.Of<IDbContextFactory<AppDbContext>>();
            hashGenerator ??= Mock.Of<IHashGenerator>();
            ldapService ??= Mock.Of<ILdapService>();

            return new UserService(logger, contextFactory, hashGenerator, ldapService);
        }
    }
}
