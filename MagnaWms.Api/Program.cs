using MagnaWms.Api.Extensions;
namespace MagnaWms.Api;

public static class Program
{
    public class Program
    {
        public static void Main(string[] args)
        {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
        builder.Services
            .AddControllers();

        builder.Services
            .AddApiVersioningWithExplorer()

        WebApplication app = builder.Build();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
