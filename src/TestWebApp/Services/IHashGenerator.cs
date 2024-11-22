namespace TestWebApp.Services
{
    public interface IHashGenerator
    {
        string GeneratePassHash(string userName, string password);
        string GenerateSession(string passHash);
    }
}
