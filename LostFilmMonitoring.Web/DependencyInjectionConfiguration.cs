// <copyright file="DependencyInjectionConfiguration.cs" company="Alexander Panfilenok">
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
    using LostFilmMonitoring.BLL;
    using LostFilmMonitoring.Common;
    using LostFilmMonitoring.DAO.DAO;
    using LostFilmTV.Client;
    using LostFilmTV.Client.RssFeed;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Responsible for adding services in DependencyInjection container.
    /// </summary>
    internal static class DependencyInjectionConfiguration
    {
        /// <summary>
        /// Extension mentod that adds services to DI container.
        /// </summary>
        /// <param name="services">IServiceCollection.</param>
        internal static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<IConfiguration, Configuration>();
            services.AddSingleton<HealthReporter>();
            services.AddSingleton<ILogger>(provider => new Logger("ROOT", provider.GetService<HealthReporter>()));
            services.AddSingleton<TorrentFileDAO>();
            services.AddSingleton<SeriesDAO>();
            services.AddSingleton<FeedDAO>();
            services.AddSingleton<UserDAO>();
            services.AddSingleton<SubscriptionDAO>();
            services.AddSingleton<TorrentFileDownloader>();
            services.AddSingleton<ReteOrgRssFeed>();
            services.AddSingleton<SeriesCoverService>();
            services.AddSingleton<RssFeedUpdaterService>();
            services.AddSingleton<Client>();
            services.AddTransient<ICurrentUserProvider, CurrentUserProvider>();
            services.AddTransient<RssFeedService>();
            services.AddTransient<PresentationService>();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
        }
    }
}
