using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LostFilmMonitoring.BLL.Interfaces;

namespace LostFilmMonitoring.BLL.Implementations
{
    public class SerialCoverService : ISerialCoverService
    {
        private readonly string _serialCoversDirectory;

        public SerialCoverService(string serialCoversDirectory)
        {
            _serialCoversDirectory = serialCoversDirectory;
        }

        private static string Filtered(string serial)
        {
            var index = serial.IndexOf('(');
            if (index > 0)
            {
                return serial.Substring(0, index).Trim();
            }

            return serial;
        }

        public async Task EnsureImageDownloaded(string serial)
        {
            var coverPath = Path.Combine(_serialCoversDirectory, serial.EscapePath() + ".jpg");
            if (File.Exists(coverPath)) return;
            using (var httpClient = new HttpClient())
            {
                var searchRequest = new HttpRequestMessage(HttpMethod.Get, $"http://www.lostfilm.tv/search/?q={System.Uri.EscapeUriString(Filtered(serial))}");
                var searchResponse = await httpClient.SendAsync(searchRequest);
                var searchContent = await searchResponse.Content.ReadAsStringAsync();
                var match = Regex.Match(searchContent, "<a href=\"/series/([^\"]+)\"");
                if (!match.Success) return;

                var seasonRequest = new HttpRequestMessage(HttpMethod.Get, $"http://www.lostfilm.tv/series/{match.Groups[1].Value}/season_1");
                var seasonResponse = await httpClient.SendAsync(seasonRequest);
                var seasonContent = await seasonResponse.Content.ReadAsStringAsync();

                match = Regex.Match(seasonContent, "//static.lostfilm.tv/Images/[\\d]+/Posters/t_shmoster_s[\\d]+.jpg");
                if (match == Match.Empty) return;

                var imageRequest = new HttpRequestMessage(HttpMethod.Get, "http:" + match.Value);
                var imageResponse = await httpClient.SendAsync(imageRequest);
                var imageStream = await imageResponse.Content.ReadAsStreamAsync();
                using (var fileStream = File.Create(coverPath))
                {
                    imageStream.Seek(0, SeekOrigin.Begin);
                    imageStream.CopyTo(fileStream);
                }
            }
        }
    }
}