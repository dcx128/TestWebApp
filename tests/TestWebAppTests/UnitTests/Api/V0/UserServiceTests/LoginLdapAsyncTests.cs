using System.Net;
using FluentAssertions;
using TestWebAppTests.UnitTests.MotherFactories;

namespace TestWebAppTests.UnitTests.Api.V0.UserServiceTests
{
    public class LoginLdapAsyncTests : UserServiceTestsBase
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
            var task = sut.LoginLdapAsync(userName, password, default);
            var taskComplete = task.Wait(callTimeout);
            var actual = await task;

            // Assert
            taskComplete.Should().BeTrue($"{nameof(sut.LoginLdapAsync)} must not hang");
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task AnErrorOccurred_ReturnsInternalServerError()
        {
            // Arrange
            var sut = CreateSut(ldapService: mf.LdapService().WithFailLoginAsync());
            var expected = sut.Problem(statusCode: (int)HttpStatusCode.InternalServerError);

            // Act
            var task = sut.LoginLdapAsync(testUserName, testUserPass, default);
            var taskComplete = task.Wait(callTimeout);
            var actual = await task;

            // Assert
            taskComplete.Should().BeTrue($"{nameof(sut.LoginLdapAsync)} must not hang");
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task LoginFailed_ReturnsForbid()
        {
            // Arrange
            var sut = CreateSut(ldapService: mf.LdapService().WithLoginAsyncReturns(false));
            var expected = sut.Forbid();

            // Act
            var task = sut.LoginLdapAsync(testUserName, testUserPass, default);
            var taskComplete = task.Wait(callTimeout);
            var actual = await task;

            // Assert
            taskComplete.Should().BeTrue($"{nameof(sut.LoginLdapAsync)} must not hang");
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task LoginSuccessful_ReturnsOk()
        {
            // Arrange
            var sut = CreateSut(ldapService: mf.LdapService().WithLoginAsyncReturns(true));
            var expected = sut.Ok();

            // Act
            var task = sut.LoginLdapAsync(testUserName, testUserPass, default);
            var taskComplete = task.Wait(callTimeout);
            var actual = await task;

            // Assert
            taskComplete.Should().BeTrue($"{nameof(sut.LoginLdapAsync)} must not hang");
            actual.Should().BeEquivalentTo(expected);
        }
    }
}
