namespace TestWebApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using var app = WebApplication
                .CreateBuilder(args)
                .ConfigureServices() // Add services to the container
                .Build();

            app
                .If(app.Environment.IsDevelopment(), then => then.UseSwagger().UseSwaggerUI()) // Configure the HTTP request pipeline
                .UseHttpsRedirection()
                .UseAuthorization();

            app.MapControllers();

            await app.RunAsync();
        }
    }
}
