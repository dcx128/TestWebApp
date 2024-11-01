namespace TestWebApp.Settings
{
    public interface IProductSettings
    {
        ADSettings ADSettings { get; set; }
        string DbConnectionString { get; set; }
    }
}
