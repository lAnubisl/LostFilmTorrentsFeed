// <copyright file="Startup.cs" company="Alexander Panfilenok">
// MIT License
// Copyright (c) 2021 Alexander Panfilenok
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the 'Software'), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// </copyright>

namespace LostFilmMonitoring.Web
{
    using System.Threading.Tasks;
    using HealthChecks.UI.Client;
    using LostFilmMonitoring.BLL;
    using LostFilmMonitoring.Common;
    using LostFilmMonitoring.Updater;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Sentry.AspNetCore;

    /// <summary>
    /// Startup.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// ConfigureServices.
        /// </summary>
        /// <param name="services">IServiceCollection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddServices();
            services.AddHealthChecks()
                .AddCheck<LastExceptionHealthCheck>("Last Exception");
            services.AddControllersWithViews();
            services.AddHttpClient();
        }

        /// <summary>
        /// Configure.
        /// </summary>
        /// <param name="app">IApplicationBuilder.</param>
        /// <param name="env">IWebHostEnvironment.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            (app.ApplicationServices.GetService<IConfiguration>() as Configuration).Init(env.ContentRootPath);
            app.UseExceptionHandler(new ExceptionHandlerOptions()
            {
                ExceptionHandler = (ctx) =>
                {
                    var feature = ctx.Features.Get<IExceptionHandlerFeature>();
                    app.ApplicationServices.GetService<ILogger>().Log(feature.Error);
                    return Task.FromResult(0);
                },
            });
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
            });
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSentryTracing();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health", new HealthCheckOptions()
                {
                    ResultStatusCodes =
                    {
                        [HealthStatus.Healthy] = StatusCodes.Status200OK,
                        [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
                        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
                    },
                    AllowCachingResponses = false,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
                });
            });
            app.ApplicationServices.GetService<ILogger>().Info("Application started.");
            UpdateFeedsJobRunner.Schedule(app.ApplicationServices);
        }
    }
}
