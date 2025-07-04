using BuildingBlocks.Exceptions.Handler;
using Carter;

namespace Ordering.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)  //this is an extension method for IServiceCollection type of vars.
        {
            services.AddCarter();
            services.AddExceptionHandler<CustomExceptionHandler>();
            services.AddHealthChecks();

            return services;
        }

        public static WebApplication UseApiServices(this WebApplication app)   //this is an extension method for WebApplication type of vars.
        {
            app.MapCarter();
            app.UseExceptionHandler(options => { });
            app.UseHealthChecks("/health");

            return app;
        }
    }
}
