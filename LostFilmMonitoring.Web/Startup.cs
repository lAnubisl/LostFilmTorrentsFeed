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
    using System;
    using System.Threading.Tasks;
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

    /// <summary>
    /// Startup.
    /// </summary>
    public class Startup
    {
        private static IConfiguration configuration;
        private static ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">IConfiguration.</param>
        public Startup(IConfiguration configuration)
        {
            Startup.configuration = configuration;
        }

        /// <summary>
        /// Gets Configuration.
        /// </summary>
        public static IConfiguration Configuration => configuration;

        /// <summary>
        /// ConfigureServices.
        /// </summary>
        /// <param name="services">IServiceCollection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureLogger();
            logger = new Logger("Root");
            services.AddTransient<PresentationService>();
            services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ILogger>(logger);
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Latest)
                .AddMvcOptions(o => { o.EnableEndpointRouting = false; });
            logger.Info("Application started.");
        }

        /// <summary>
        /// Configure.
        /// </summary>
        /// <param name="app">IApplicationBuilder.</param>
        /// <param name="env">IWebHostEnvironment.</param>
#pragma warning disable IDE0060 // Remove unused parameter
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            BLL.Configuration.Init(
                Environment.GetEnvironmentVariable("BASEPATH") ?? env.ContentRootPath,
                Environment.GetEnvironmentVariable("BASEURL") ?? "http://localhost:5000",
                Environment.GetEnvironmentVariable("BASEFEEDCOOKIE") ?? "58eaf77d5fc3eeda277449d3a3cd9a4a.1874597");
            app.UseExceptionHandler(new ExceptionHandlerOptions()
            {
                ExceptionHandler = (ctx) =>
                {
                    var feature = ctx.Features.Get<IExceptionHandlerFeature>();
                    logger.Log(feature.Error);
                    return Task.FromResult(0);
                },
            });
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
            });
            app.UseStaticFiles();
            app.UseMvc();
        }

        private static void ConfigureLogger()
        {
            var minLogLevel = "Debug";
            var maxLogLevel = "Fatal";
            LoggerConfiguration.ConfigureLogger(minLogLevel, maxLogLevel);
        }
    }
}
