using Microsoft.EntityFrameworkCore;
using Moq;
using TestWebApp.Model;
using TestWebApp.Model.Types;
using TestWebAppTests.UnitTests.Base;

namespace TestWebAppTests.UnitTests.MotherFactories
{
    public static class DbContextFactoryMotherFactory
    {
        public static IDbContextFactory<AppDbContext> DbContextFactory(this MotherFactory _)
        {
            var dbContextFactoryMock = new Mock<IDbContextFactory<AppDbContext>>();
            return dbContextFactoryMock.Object;
        }

        public static IDbContextFactory<AppDbContext> WithFailCreateDbContextAsync(
            this IDbContextFactory<AppDbContext> dbContextFactory)
        {
            var dbContextFactoryMock = Mock.Get(dbContextFactory);

            dbContextFactoryMock
                .Setup(m => m.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                .Throws<Exception>();

            return dbContextFactoryMock.Object;
        }

        public static IDbContextFactory<AppDbContext> WithSaveChangesAsyncReturns(
            this IDbContextFactory<AppDbContext> dbContextFactory,
            int result)
        {
            /*var dbContextFactoryMock = Mock.Get(dbContextFactory);

            dbContextFactoryMock
                .Setup(m => m.CreateDbContext())
                .Returns(() => new Mock<AppDbContext>().Object);*/

            using (var context = dbContextFactory.CreateDbContextAsync(It.IsAny<CancellationToken>()).GetAwaiter().GetResult())
            {
                var appDbContextMock = Mock.Get(context);
                appDbContextMock
                    .Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(result);

                context.SaveChanges();
            }

            return dbContextFactory;
        }

        public static IDbContextFactory<AppDbContext> WithThrowIfCancellationRequested(
            this IDbContextFactory<AppDbContext> dbContextFactory,
            CancellationToken cancellationToken)
        {
            var dbContextFactoryMock = Mock.Get(dbContextFactory);

            dbContextFactoryMock
                .Setup(m => m.CreateDbContextAsync(cancellationToken))
                .Callback<CancellationToken>(t => t.ThrowIfCancellationRequested());

            return dbContextFactoryMock.Object;
        }

        public static IDbContextFactory<AppDbContext> WithUsers(
            this IDbContextFactory<AppDbContext> dbContextFactory,
            IEnumerable<User> users)
        {
            var dbContextFactoryMock = Mock.Get(dbContextFactory);

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("InMemoryDB")
                .Options;

            using (var context = new AppDbContext(options))
            {
                foreach (var user in users)
                {
                    context.Users.Add(user);
                }

                context.SaveChanges();
            }

            dbContextFactoryMock
                .Setup(m => m.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new AppDbContext(options));

            return dbContextFactoryMock.Object;
        }
    }
}
