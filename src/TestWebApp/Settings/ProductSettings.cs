namespace TestWebApp.Settings
{
    public class ProductSettings : IProductSettings
    {
        public ADSettings ADSettings { get; set; } = null!;
        public string DbConnectionString { get; set; } = null!;
    }
}
