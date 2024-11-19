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
        public void UserNameOrPasswordIsNullOrWhiteSpace_ReturnsBadRequest(string userName, string password)
        {
            // Arrange
            var sut = CreateSut();
            var expected = sut.BadRequest();

            // Act
            var task = sut.LoginLdapAsync(userName, password, CancellationToken.None);
            var taskComplete = task.Wait(callTimeout);
            var actual = task.GetAwaiter().GetResult();

            // Assert
            taskComplete.Should().BeTrue($"{nameof(sut.LoginLdapAsync)} must not hang");
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void AnErrorOccurred_ReturnsInternalServerError()
        {
            // Arrange
            var sut = CreateSut(ldapService: mf.FaultedLdapService());
            var expected = sut.Problem(statusCode: (int)HttpStatusCode.InternalServerError);

            // Act
            var task = sut.LoginLdapAsync(testUserName, testUserPass, CancellationToken.None);
            var taskComplete = task.Wait(callTimeout);
            var actual = task.GetAwaiter().GetResult();

            // Assert
            taskComplete.Should().BeTrue($"{nameof(sut.LoginLdapAsync)} must not hang");
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void LoginFailed_ReturnsForbid()
        {
            // Arrange
            var sut = CreateSut(ldapService: mf.LdapService(loginResult: false));
            var expected = sut.Forbid();

            // Act
            var task = sut.LoginLdapAsync(testUserName, testUserPass, CancellationToken.None);
            var taskComplete = task.Wait(callTimeout);
            var actual = task.GetAwaiter().GetResult();

            // Assert
            taskComplete.Should().BeTrue($"{nameof(sut.LoginLdapAsync)} must not hang");
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void LoginSuccessful_ReturnsOk()
        {
            // Arrange
            var sut = CreateSut(ldapService: mf.LdapService(loginResult: true));
            var expected = sut.Ok();

            // Act
            var task = sut.LoginLdapAsync(testUserName, testUserPass, CancellationToken.None);
            var taskComplete = task.Wait(callTimeout);
            var actual = task.GetAwaiter().GetResult();

            // Assert
            taskComplete.Should().BeTrue($"{nameof(sut.LoginLdapAsync)} must not hang");
            actual.Should().BeEquivalentTo(expected);
        }
    }
}
