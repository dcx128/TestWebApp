using System.Net;
using FluentAssertions;
using TestWebAppTests.UnitTests.MotherFactories;

namespace TestWebAppTests.UnitTests.Api.V0.UserServiceTests
{
    public class CheckinAsyncTests : UserServiceTestsBase
    {
        [TestCase("", "")]
        [TestCase("", "   ")]
        [TestCase("   ", "")]
        [TestCase("   ", "   ")]
        public async Task UserNameOrPasswordIsNullOrWhiteSpace_ReturnsBadRequest(string userName, string password)
        {
            // Arrange
            var sut = CreateSut();
            var expected = sut.BadRequest();

            // Act
            var task = sut.CheckinAsync(userName, password, default);
            var taskComplete = task.Wait(callTimeout);
            var actual = await task;

            // Assert
            taskComplete.Should().BeTrue($"{nameof(sut.CheckinAsync)} must not hang");
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
            var task = sut.CheckinAsync(testUserName, testUserPass, default);
            var taskComplete = task.Wait(callTimeout);
            var actual = await task;

            // Assert
            taskComplete.Should().BeTrue($"{nameof(sut.CheckinAsync)} must not hang");
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task OperationCanceled_ReturnsNoContent()
        {
            // Arrange
            var tokenSource = new CancellationTokenSource();
            var sut = CreateSut(contextFactory: mf.DbContextFactory()
                .WithThrowIfCancellationRequested(tokenSource.Token));
            var expected = sut.NoContent();

            // Act
            tokenSource.Cancel();
            var task = sut.CheckinAsync(testUserName, testUserPass, tokenSource.Token);
            var taskComplete = task.Wait(callTimeout);
            var actual = await task;

            // Assert
            taskComplete.Should().BeTrue($"{nameof(sut.CheckinAsync)} must not hang");
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task UserNameExist_ReturnsConflict()
        {
            // Arrange
            var sut = CreateSut(contextFactory: mf.DbContextFactory()
                .WithUsers(testUser));
            var expected = sut.Conflict();

            // Act
            var task = sut.CheckinAsync(testUserName, testUserPass, default);
            var taskComplete = task.Wait(callTimeout);
            var actual = await task;

            // Assert
            taskComplete.Should().BeTrue($"{nameof(sut.CheckinAsync)} must not hang");
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task CheckinFailed_ReturnsBadRequest()
        {
            // Arrange
            var sut = CreateSut(
                contextFactory: mf.DbContextFactory()
                    .WithUsers([], saveChangesAsyncReturns: 0),
                hashGenerator: mf.HashGenerator()
                    .WithPassHash(testUserName, testUserPass, testUserPassHash)
                    .WithSession(testUserPassHash, testSession));
            var expected = sut.BadRequest();

            // Act
            var task = sut.CheckinAsync(testUserName, testUserPass, default);
            var taskComplete = task.Wait(callTimeout);
            var actual = await task;

            // Assert
            taskComplete.Should().BeTrue($"{nameof(sut.CheckinAsync)} must not hang");
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task CheckinSuccessful_ReturnsOk()
        {
            // Arrange
            var sut = CreateSut(
                contextFactory: mf.DbContextFactory()
                    .WithUsers([], saveChangesAsyncReturns: 1),
                hashGenerator: mf.HashGenerator()
                    .WithPassHash(testUserName, testUserPass, testUserPassHash)
                    .WithSession(testUserPassHash, testSession));
            var expected = sut.Ok();

            // Act
            var task = sut.CheckinAsync(testUserName, testUserPass, default);
            var taskComplete = task.Wait(callTimeout);
            var actual = await task;

            // Assert
            taskComplete.Should().BeTrue($"{nameof(sut.CheckinAsync)} must not hang");
            actual.Should().BeEquivalentTo(expected);
        }
    }
}
