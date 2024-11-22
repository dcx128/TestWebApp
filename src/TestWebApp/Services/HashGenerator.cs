namespace TestWebApp.Services
{
    public class HashGenerator(IHashService hashService) : IHashGenerator
    {
        public string GeneratePassHash(string userName, string password) => hashService.GetHash($"{userName}:{password}");
        public string GenerateSession(string passHash) => hashService.GetHash($"{passHash}+{DateTime.UtcNow}");
    }
}
