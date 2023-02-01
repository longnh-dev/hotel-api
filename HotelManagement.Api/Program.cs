using Autofac.Extensions.DependencyInjection;
using HotelManagement.Domain.Common;
using HotelManagement.Infrastructure;
using Serilog;
namespace HotelManagement.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = ConfigCollection.Instance.GetConfiguration();
            Log.Logger = new LoggerConfiguration()
                .CreateLogger();

            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetService<HotelDbContext>();

                // context.Database.Migrate();
                // context.Database.EnsureCreated();
            }
            CreateHostBuilder(args).Build().Run();

            Log.Information("Hotels API started.");
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
               .UseServiceProviderFactory(new AutofacServiceProviderFactory())
               .ConfigureWebHostDefaults(webBuilder =>
               {
                   webBuilder.UseStartup<Startup>();
               });
    }
}
