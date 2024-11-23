using System.Net;
using FluentAssertions;
using TestWebAppTests.UnitTests.MotherFactories;

namespace TestWebAppTests.UnitTests.Api.V0.UserServiceTests
{
    public class LoginAsyncTests : UserServiceTestsBase
    {
        [TestCase("", "")]
        [TestCase("", "   ")]
        [TestCase("   ", "")]
        [TestCase("   ", "   ")]
        public async Task UserNameOrPasswordIsNullOrWhiteSpace_ReturnsForbid(string userName, string password)
        {
            // Arrange
            var sut = CreateSut();
            var expected = sut.Forbid();

            // Act
            var task = sut.LoginAsync(userName, password, default);
            var taskComplete = task.Wait(callTimeout);
            var actual = await task;

            // Assert
            taskComplete.Should().BeTrue($"{nameof(sut.LoginAsync)} must not hang");
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
            var task = sut.LoginAsync(testUserName, testUserPass, default);
            var taskComplete = task.Wait(callTimeout);
            var actual = await task;

            // Assert
            taskComplete.Should().BeTrue($"{nameof(sut.LoginAsync)} must not hang");
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
            var task = sut.LoginAsync(testUserName, testUserPass, tokenSource.Token);
            var taskComplete = task.Wait(callTimeout);
            var actual = await task;

            // Assert
            taskComplete.Should().BeTrue($"{nameof(sut.LoginAsync)} must not hang");
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task UserNotExists_ReturnsNotFound()
        {
            // Arrange
            var sut = CreateSut(contextFactory: mf.DbContextFactory()
                .WithNoUsers());
            var expected = sut.NotFound();

            // Act
            var task = sut.LoginAsync(testUserName, testUserPass, default);
            var taskComplete = task.Wait(callTimeout);
            var actual = await task;

            // Assert
            taskComplete.Should().BeTrue($"{nameof(sut.LoginAsync)} must not hang");
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task PassHashNotEqual_ReturnsForbid()
        {
            // Arrange
            const string wrongPassword = $"{testUserPass}+0";
            var sut = CreateSut(
                contextFactory: mf.DbContextFactory()
                    .WithUsers(testUser),
                hashGenerator: mf.HashGenerator()
                    .WithPassHash(userName: testUserName, password: testUserPass, passHash: testUserPassHash));
            var expected = sut.Forbid();

            // Act
            var task = sut.LoginAsync(testUserName, wrongPassword, default);
            var taskComplete = task.Wait(callTimeout);
            var actual = await task;

            // Assert
            taskComplete.Should().BeTrue($"{nameof(sut.LoginAsync)} must not hang");
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task LoginFailed_ReturnsBadRequest()
        {
            // Arrange
            var sut = CreateSut(
                contextFactory: mf.DbContextFactory()
                    .WithUsers(testUser, saveChangesAsyncReturns: 0),
                hashGenerator: mf.HashGenerator()
                    .WithPassHash(userName: testUserName, password: testUserPass, passHash: testUserPassHash));
            var expected = sut.BadRequest();

            // Act
            var task = sut.LoginAsync(testUserName, testUserPass, default);
            var taskComplete = task.Wait(callTimeout);
            var actual = await task;

            // Assert
            taskComplete.Should().BeTrue($"{nameof(sut.LoginAsync)} must not hang");
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task LoginSuccessful_ReturnsOk()
        {
            // Arrange
            var sut = CreateSut(
                contextFactory: mf.DbContextFactory()
                    .WithUsers(testUser, saveChangesAsyncReturns: 1),
                hashGenerator: mf.HashGenerator()
                    .WithPassHash(userName: testUserName, password: testUserPass, passHash: testUserPassHash));
            var expected = sut.Ok();

            // Act
            var task = sut.LoginAsync(testUserName, testUserPass, default);
            var taskComplete = task.Wait(callTimeout);
            var actual = await task;

            // Assert
            taskComplete.Should().BeTrue($"{nameof(sut.LoginAsync)} must not hang");
            actual.Should().BeEquivalentTo(expected);
        }
    }
}
