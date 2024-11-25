using Moq;
using TestWebApp.Services;
using TestWebAppTests.UnitTests.Base;

namespace TestWebAppTests.UnitTests.MotherFactories
{
    public static class LdapServiceMotherFactory
    {
        public static ILdapService LdapService(this MotherFactory _)
        {
            var ldapServiceMock = new Mock<ILdapService>();
            return ldapServiceMock.Object;
        }

        public static ILdapService WithFailLoginAsync(this ILdapService ldapService)
        {
            var ldapServiceMock = Mock.Get(ldapService);

            ldapServiceMock
                .Setup(m => m.LoginAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Throws<Exception>();

            return ldapServiceMock.Object;
        }

        public static ILdapService WithLoginAsyncReturns(this ILdapService ldapService, bool loginResult)
        {
            var ldapServiceMock = Mock.Get(ldapService);

            ldapServiceMock
                .Setup(m => m.LoginAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(loginResult);

            return ldapServiceMock.Object;
        }
    }
}
