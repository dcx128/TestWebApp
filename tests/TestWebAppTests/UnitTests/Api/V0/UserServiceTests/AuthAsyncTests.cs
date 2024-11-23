using System.Net;
using FluentAssertions;
using TestWebAppTests.UnitTests.MotherFactories;

namespace TestWebAppTests.UnitTests.Api.V0.UserServiceTests
{
    public class AuthAsyncTests : UserServiceTestsBase
    {
        [TestCase("", "")]
        [TestCase("", "   ")]
        [TestCase("   ", "")]
        [TestCase("   ", "   ")]
        public async Task UserNameOrSessionIsNullOrWhiteSpace_ReturnsForbid(string userName, string session)
        {
            // Arrange
            var sut = CreateSut();
            var expected = sut.Forbid();

            // Act
            var task = sut.AuthAsync(userName, session, default);
            var taskComplete = task.Wait(callTimeout);
            var actual = await task;

            // Assert
            taskComplete.Should().BeTrue($"{nameof(sut.AuthAsync)} must not hang");
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task AnErrorOccurred_ReturnsInternalServerError()
        {
            // Arrange
            var sut = CreateSut(contextFactory: mf.DbContextFactory()
                .WithFailCreateDbContextAsync());
            var expected = sut.Problem(statusCode: (int)HttpStatusCode.InternalServerError);

            // Act
            var task = sut.AuthAsync(testUserName, testSession, default);
            var taskComplete = task.Wait(callTimeout);
            var actual = await task;

            // Assert
            taskComplete.Should().BeTrue($"{nameof(sut.AuthAsync)} must not hang");
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task OperationCanceled_ReturnsForbid()
        {
            // Arrange
            var tokenSource = new CancellationTokenSource();
            var sut = CreateSut(contextFactory: mf.DbContextFactory()
                .WithThrowIfCancellationRequested(tokenSource.Token));
            var expected = sut.Forbid();

            // Act
            tokenSource.Cancel();
            var task = sut.AuthAsync(testUserName, testSession, tokenSource.Token);
            var taskComplete = task.Wait(callTimeout);
            var actual = await task;

            // Assert
            taskComplete.Should().BeTrue($"{nameof(sut.AuthAsync)} must not hang");
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task UserNotExists_ReturnsForbid()
        {
            // Arrange
            var sut = CreateSut(contextFactory: mf.DbContextFactory()
                .WithNoUsers());
            var expected = sut.Forbid();

            // Act
            var task = sut.AuthAsync(testUserName, testSession, default);
            var taskComplete = task.Wait(callTimeout);
            var actual = await task;

            // Assert
            taskComplete.Should().BeTrue($"{nameof(sut.AuthAsync)} must not hang");
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task SessionNotEqual_ReturnsForbid()
        {
            // Arrange
            const string wrongSession = $"{testSession}+0";
            var sut = CreateSut(contextFactory: mf.DbContextFactory()
                .WithUsers(testUser));
            var expected = sut.Forbid();

            // Act
            var task = sut.AuthAsync(testUserName, wrongSession, default);
            var taskComplete = task.Wait(callTimeout);
            var actual = await task;

            // Assert
            taskComplete.Should().BeTrue($"{nameof(sut.AuthAsync)} must not hang");
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task AuthSuccessful_ReturnsOk()
        {
            // Arrange
            var sut = CreateSut(contextFactory: mf.DbContextFactory()
                .WithUsers(testUser));
            var expected = sut.Ok();

            // Act
            var task = sut.AuthAsync(testUserName, testSession, default);
            var taskComplete = task.Wait(callTimeout);
            var actual = await task;

            // Assert
            taskComplete.Should().BeTrue($"{nameof(sut.AuthAsync)} must not hang");
            actual.Should().BeEquivalentTo(expected);
        }
    }
}
