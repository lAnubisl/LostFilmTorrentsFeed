using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LostFilmMonitoring.BLL.Implementations.RssFeedService;
using LostFilmMonitoring.BLL.Interfaces;
using LostFilmMonitoring.BLL.Models;
using LostFilmMonitoring.Common;
using Newtonsoft.Json;

namespace LostFilmMonitoring.BLL.Implementations
{
    public class LostFilmRegistrationService : ILostFilmRegistrationService
    {
        private readonly ILogger _logger;

        public LostFilmRegistrationService(ILogger logger)
        {
            _logger = logger.CreateScope(nameof(LostFilmRegistrationService));
        }

        public async Task<CaptchaViewModel> GetNewCaptcha()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "http://www.lostfilm.tv/simple_captcha.php");
            using (var client = new HttpClient())
            {
                var response = await client.SendAsync(request);
                var setCookieHeader = response.Headers.Where(h => h.Key == "Set-Cookie").Select(h => h.Value).FirstOrDefault()?.First();

                if (setCookieHeader == null)
                {
                    throw new Exception();
                }
                var cookieKey = setCookieHeader.Substring(setCookieHeader.IndexOf("=") + 1);
                cookieKey = cookieKey.Substring(0, cookieKey.IndexOf(";"));
                var content = await response.Content.ReadAsByteArrayAsync();
                return new CaptchaViewModel(cookieKey, content);
            }
        }

        public async Task<RegistrationResultModel> Register(string captchaCookie, string captcha)
        {
            var guid = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16);
            var request = new HttpRequestMessage(HttpMethod.Post, "http://www.lostfilm.tv/ajaxik.php");
            request.Headers.Add("Cookie", $"PHPSESSID={captchaCookie}");
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>() {
                { "act", "users" },
                { "type", "signup" },
                { "login", guid },
                { "mail", $"{guid}@gmail.com" },
                { "pass", guid },
                { "pass_check", guid },
                { "rem", "1" },
                { "captcha", captcha}
            });
            using (var client = new HttpClient())
            {
                var response = await client.SendAsync(request);
                var responseJson = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<RegistrationResponseModel>(responseJson);
                if (!result.success)
                {
                    return new RegistrationResultModel() { Error = "Судя по всему, число введено не верно." };
                }

                var cookie = response.Headers.Where(h => h.Key == "Set-Cookie").Select(h => h.Value).FirstOrDefault()?.Last();
                cookie = cookie.Substring(cookie.IndexOf("=") + 1);
                cookie = cookie.Substring(0, cookie.IndexOf(";"));
                var userIds = await GetUserIds(cookie);
                return new RegistrationResultModel(cookie, userIds.Usess, userIds.Uid);
            }
        }

        public async Task<UserIds> GetUserIds(string cookie)
        {
            var lostFilmRssFeedService = new LostFilmRssFeedService(_logger);
            var items = await lostFilmRssFeedService.LoadFeedItems();
            if (items == null || !items.Any())
            {
                return null;
            }

            using (var client = new HttpClient())
            {
                var episodeResponse = await client.SendAsync(AddCookies(new HttpRequestMessage(HttpMethod.Get, items.First().Link.Replace("lostfilm.tv", "www.lostfilm.tv")), cookie));
                var episodeResponseContent = await episodeResponse.Content.ReadAsStringAsync();
                var episodeIdMatch = Regex.Match(episodeResponseContent, "PlayEpisode\\('(\\d+)'\\)");
                if (!episodeIdMatch.Success)
                {
                    _logger.Error($"Cannot get episodeId from: {Environment.NewLine}{episodeResponseContent}");
                    return null;
                }

                var episodeId = episodeIdMatch.Groups[1].Value;
                var linkResponse = await client.SendAsync(AddCookies(new HttpRequestMessage(HttpMethod.Get, $"https://www.lostfilm.tv/v_search.php?a={episodeId}"), cookie));
                var linkResponseBytes = await linkResponse.Content.ReadAsByteArrayAsync();
                var linkResponseContent = Encoding.UTF8.GetString(linkResponseBytes);

                var linkMatch = Regex.Match(linkResponseContent, "url=([^\"]+)");
                if (!linkMatch.Success)
                {
                    _logger.Error($"Cannot get link from: {Environment.NewLine}{linkResponseContent}");
                    return null;
                }

                var link = linkMatch.Groups[1].Value;
                var uidMatch = Regex.Match(link, "u=([^&]+)");
                if (!uidMatch.Success)
                {
                    _logger.Error($"Cannot get uid from: {Environment.NewLine}{link}");
                    return null;
                }

                var uId = uidMatch.Groups[1].Value;
                var usessResponse = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, link));
                var usessResponseContent = await usessResponse.Content.ReadAsStringAsync();
                var usessMatch = Regex.Match(usessResponseContent, "this.innerHTML = '([^']+)'");
                if (!usessMatch.Success)
                {
                    _logger.Error($"Cannot get usess from: {Environment.NewLine}{usessResponseContent}");
                    return null;
                }

                var usess = usessMatch.Groups[1].Value;
                return new UserIds
                {
                    Uid = uId,
                    Usess = usess
                };
            }
        }

        private static HttpRequestMessage AddCookies(HttpRequestMessage httpRequestMessage, string cookie)
        {
            httpRequestMessage.Headers.Add("Cookie", $"lf_session={cookie}");
            return httpRequestMessage;
        }
    }
}