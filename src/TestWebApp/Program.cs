namespace TestWebApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using var app = WebApplication
                .CreateBuilder(args)
                .ConfigureServices(args) // Add services to the container
                .Build();

            try
            {
                app
                    .If(app.Environment.IsDevelopment(), then => then.UseSwagger().UseSwaggerUI()) // Configure the HTTP request pipeline
                    .UseHttpsRedirection()
                    .UseAuthorization();

                app.MapControllers();
            }
            catch (Exception ex)
            {
                app.Logger.LogError($"Fatal error: '{ex.Message}'");
            }
            finally
            {
                await app.RunAsync();
            }
        }
    }
}
