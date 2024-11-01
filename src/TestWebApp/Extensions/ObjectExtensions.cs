using Newtonsoft.Json;

namespace TestWebApp.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToJson(this object obj) => JsonConvert.SerializeObject(obj);
    }
}
