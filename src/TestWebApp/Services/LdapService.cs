using System.DirectoryServices.Protocols;
using System.Net;
using TestWebApp.Settings;

namespace TestWebApp.Services
{
    public class LdapService(IProductSettings productSettings) : ILdapService
    {
        public async Task<bool> LoginAsync(string userName, string password, CancellationToken cancellationToken)
        {
            try
            {
                return await Task.Run(() =>
                {
                    try
                    {
                        var id = new LdapDirectoryIdentifier(productSettings.ADSettings.Host, productSettings.ADSettings.Port);
                        using var connection = new LdapConnection(id);
                        connection.AuthType = productSettings.ADSettings.AuthType;
                        connection.SessionOptions.ProtocolVersion = productSettings.ADSettings.ProtocolVersion;
                        var credentials = new NetworkCredential(userName, password);
                        connection.Bind(credentials);
                        return true;
                    }
                    catch (LdapException)
                    {
                        return false;
                    }
                }, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                return false;
            }
        }
    }
}
