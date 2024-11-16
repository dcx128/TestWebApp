namespace TestWebApp.Services
{
    public interface ILdapService
    {
        Task<bool> LoginAsync(string userName, string password, CancellationToken cancellationToken);
    }
}
