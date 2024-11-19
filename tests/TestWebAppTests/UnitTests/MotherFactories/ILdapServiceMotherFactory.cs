using Moq;
using TestWebApp.Services;
using TestWebAppTests.UnitTests.Base;

namespace TestWebAppTests.UnitTests.MotherFactories
{
    public static class ILdapServiceMotherFactory
    {
        public static ILdapService FaultedLdapService(this MotherFactory _)
        {
            var ldapServiceMock = new Mock<ILdapService>();

            ldapServiceMock
                .Setup(m => m.LoginAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Throws<Exception>();

            return ldapServiceMock.Object;
        }

        public static ILdapService LdapService(this MotherFactory _, bool loginResult)
        {
            var ldapServiceMock = new Mock<ILdapService>();

            ldapServiceMock
                .Setup(m => m.LoginAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(loginResult));

            return ldapServiceMock.Object;
        }
    }
}
