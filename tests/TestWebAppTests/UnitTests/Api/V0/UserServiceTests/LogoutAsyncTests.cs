using System.Net;
using FluentAssertions;
using TestWebAppTests.UnitTests.MotherFactories;

namespace TestWebAppTests.UnitTests.Api.V0.UserServiceTests
{
    public class LogoutAsyncTests : UserServiceTestsBase
    {
        [TestCase("", "")]
        [TestCase("", "   ")]
        [TestCase("   ", "")]
        [TestCase("   ", "   ")]
        public async Task UserNameOrSessionIsNullOrWhiteSpace_ReturnsBadRequest(string userName, string session)
        {
            // Arrange
            var sut = CreateSut();
            var expected = sut.BadRequest();

            // Act
            var task = sut.LogoutAsync(userName, session, default);
            var taskComplete = task.Wait(callTimeout);
            var actual = await task;

            // Assert
            taskComplete.Should().BeTrue($"{nameof(sut.LogoutAsync)} must not hang");
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task AnErrorOccurred_ReturnsInternalServerError()
        {
            // Arrange
            var sut = CreateSut(contextFactory: mf.DbContextFactory().WithFailCreateDbContextAsync());
            var expected = sut.Problem(statusCode: (int)HttpStatusCode.InternalServerError);

            // Act
            var task = sut.LogoutAsync(testUserName, testSession, default);
            var taskComplete = task.Wait(callTimeout);
            var actual = await task;

            // Assert
            taskComplete.Should().BeTrue($"{nameof(sut.LogoutAsync)} must not hang");
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task OperationCanceled_ReturnsNoContent()
        {
            // Arrange
            var tokenSource = new CancellationTokenSource();
            var sut = CreateSut(contextFactory: mf.DbContextFactory().WithThrowIfCancellationRequested(tokenSource.Token));
            var expected = sut.NoContent();

            // Act
            tokenSource.Cancel();
            var task = sut.LogoutAsync(testUserName, testSession, tokenSource.Token);
            var taskComplete = task.Wait(callTimeout);
            var actual = await task;

            // Assert
            taskComplete.Should().BeTrue($"{nameof(sut.LogoutAsync)} must not hang");
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task UserNotExists_ReturnsNotFound()
        {
            // Arrange
            var sut = CreateSut(contextFactory: mf.DbContextFactory().WithUsers(noUsers));
            var expected = sut.NotFound();

            // Act
            var task = sut.LogoutAsync(testUserName, testSession, default);
            var taskComplete = task.Wait(callTimeout);
            var actual = await task;

            // Assert
            taskComplete.Should().BeTrue($"{nameof(sut.LogoutAsync)} must not hang");
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        [Ignore("TODO")]
        public async Task SessionNotEqual_ReturnsForbid()
        {
            // Arrange
            const string wrongSession = $"{testSession}+0";
            var sut = CreateSut(contextFactory: mf.DbContextFactory().WithUsers(testUser));
            var expected = sut.Forbid();

            // Act
            var task = sut.LogoutAsync(testUserName, wrongSession, default);
            var taskComplete = task.Wait(callTimeout);
            var actual = await task;

            // Assert
            taskComplete.Should().BeTrue($"{nameof(sut.LogoutAsync)} must not hang");
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        [Ignore("TODO")]
        public async Task LogoutFailed_ReturnsBadRequest()
        {
            // Arrange
            var sut = CreateSut(contextFactory: mf.DbContextFactory().WithUsers(testUser).WithSaveChangesAsyncReturns(0));
            var expected = sut.BadRequest();

            // Act
            var task = sut.LogoutAsync(testUserName, testSession, default);
            var taskComplete = task.Wait(callTimeout);
            var actual = await task;

            // Assert
            taskComplete.Should().BeTrue($"{nameof(sut.LogoutAsync)} must not hang");
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        [Ignore("TODO")]
        public async Task LogoutSuccessful_ReturnsOk()
        {
            // Arrange
            var sut = CreateSut(contextFactory: mf.DbContextFactory().WithUsers(testUser).WithSaveChangesAsyncReturns(1));
            var expected = sut.Ok();

            // Act
            var task = sut.LogoutAsync(testUserName, testSession, default);
            var taskComplete = task.Wait(callTimeout);
            var actual = await task;

            // Assert
            taskComplete.Should().BeTrue($"{nameof(sut.LogoutAsync)} must not hang");
            actual.Should().BeEquivalentTo(expected);
        }
    }
}
