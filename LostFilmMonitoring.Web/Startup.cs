using LostFilmMonitoring.BLL;
using LostFilmMonitoring.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace LostFilmMonitoring.Web
{
    public class Startup
    {
        private static IConfiguration _configuration;
        private static ILogger _logger;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static IConfiguration Configuration => _configuration;

        private static void ConfigureLogger()
        {
            var minLogLevel = "Debug";
            var maxLogLevel = "Fatal";
            LoggerConfiguration.ConfigureLogger(minLogLevel, maxLogLevel);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureLogger();
            _logger = new Logger("Root");
            services.AddTransient<PresentationService>();
            services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ILogger>(_logger);
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Latest)
                .AddMvcOptions(o => { o.EnableEndpointRouting = false; });
            _logger.Info("Application started.");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler(new ExceptionHandlerOptions()
            {
                ExceptionHandler = (ctx) =>
                {
                    var feature = ctx.Features.Get<IExceptionHandlerFeature>();
                    _logger.Log(feature.Error);
                    return Task.FromResult(0);
                }
            });
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
