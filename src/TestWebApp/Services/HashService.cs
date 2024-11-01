using System.Security.Cryptography;
using System.Text;

namespace TestWebApp.Services
{
    public class HashService : IHashService
    {
        public string GetHash(string key)
        {
            var sb = new StringBuilder();
            foreach (var b in SHA384.HashData(Encoding.UTF8.GetBytes(key)))
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
