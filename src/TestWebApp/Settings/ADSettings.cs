using System.DirectoryServices.Protocols;

namespace TestWebApp.Settings
{
    public class ADSettings
    {
        public AuthType AuthType { get; set; } = AuthType.Basic;
        public string Host { get; set; } = null!;
        public int Port { get; set; } = 389;
        public int ProtocolVersion { get; set; } = 3;
    }
}
