using Microsoft.EntityFrameworkCore;
using Moq;
using TestWebApp.Model;
using TestWebAppTests.UnitTests.Base;

namespace TestWebAppTests.UnitTests.MotherFactories
{
    public static class IDbContextFactoryMotherFactory
    {
        public static IDbContextFactory<AppDbContext> FaultedDbContextFactory(this MotherFactory _)
        {
            var dbContextFactoryMock = new Mock<IDbContextFactory<AppDbContext>>();

            dbContextFactoryMock
                .Setup(m => m.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                .Throws<Exception>();

            return dbContextFactoryMock.Object;
        }
    }
}
