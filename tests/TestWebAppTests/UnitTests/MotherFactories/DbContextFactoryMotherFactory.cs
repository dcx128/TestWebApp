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

        public static IDbContextFactory<AppDbContext> WithNoUsers(
            this IDbContextFactory<AppDbContext> dbContextFactory)
        {
            return dbContextFactory.WithUsers([]);
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
            IEnumerable<User> users,
            int? saveChangesAsyncReturns = default)
        {
            var dbContextFactoryMock = Mock.Get(dbContextFactory);

            dbContextFactoryMock
                .Setup(m => m.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    var memdb = new DbContextOptionsBuilder<AppDbContext>()
                        .UseInMemoryDatabase(databaseName: "InMemoryDB");

                    if (saveChangesAsyncReturns.HasValue)
                    {
                        memdb = memdb.AddInterceptors(new CustomSaveChangesInterceptor(saveChangesAsyncReturns.Value));
                    }

                    var options = memdb.Options;
                    if (users.Any())
                    {
                        using var context = new AppDbContext(options);
                        context.Users.AddRange(users);
                        context.SaveChanges();
                    }

                    return new AppDbContext(options);
                });

            return dbContextFactoryMock.Object;
        }
    }
}
