using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using LostFilmMonitoring.BLL.Interfaces;
using LostFilmMonitoring.BLL.Models;
using Newtonsoft.Json;

namespace LostFilmMonitoring.BLL.Implementations
{
    public class LostFilmRegistrationService : ILostFilmRegistrationService
    {
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
                    return new RegistrationResultModel(null) { Error = "Судя по всему, число введено не верно." };
                }

                var cookie = response.Headers.Where(h => h.Key == "Set-Cookie").Select(h => h.Value).FirstOrDefault()?.Last();
                cookie = cookie.Substring(cookie.IndexOf("=") + 1);
                cookie = cookie.Substring(0, cookie.IndexOf(";"));
                return new RegistrationResultModel(cookie);
            }
        }
    }
}