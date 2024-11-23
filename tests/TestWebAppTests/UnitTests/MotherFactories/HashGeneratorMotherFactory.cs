using Moq;
using TestWebApp.Services;
using TestWebAppTests.UnitTests.Base;

namespace TestWebAppTests.UnitTests.MotherFactories
{
    public static class HashGeneratorMotherFactory
    {
        public static IHashGenerator HashGenerator(this MotherFactory _)
        {
            var hashGeneratorMock = new Mock<IHashGenerator>();
            return hashGeneratorMock.Object;
        }

        public static IHashGenerator WithPassHash(
            this IHashGenerator hashGenerator,
            string userName, string password, string passHash)
        {
            var hashGeneratorMock = Mock.Get(hashGenerator);

            hashGeneratorMock
                .Setup(m => m.GeneratePassHash(userName, password))
                .Returns(passHash);

            return hashGeneratorMock.Object;
        }
    }
}
