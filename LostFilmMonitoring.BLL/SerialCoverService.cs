using LostFilmMonitoring.DAO.DomainModels;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LostFilmMonitoring.BLL
{
    public class SerialCoverService
    {
        private readonly string _serialCoversDirectory;

        public SerialCoverService(string serialCoversDirectory)
        {
            _serialCoversDirectory = serialCoversDirectory;
        }

        public async Task EnsureImageDownloaded(FeedItem feedItem)
        {
            var serial = feedItem.Serial();
            var coverPath = Path.Combine(_serialCoversDirectory, serial.EscapePath() + ".jpg");
            if (File.Exists(coverPath)) return;
            var pageUri = feedItem.Link.Substring(0, feedItem.Link.IndexOf("season_")) + "seasons";
            using (var httpClient = new HttpClient())
            {
                var pageRequest = new HttpRequestMessage(HttpMethod.Get, pageUri);
                var pageResponse = await httpClient.SendAsync(pageRequest);
                var pageContent = await pageResponse.Content.ReadAsStringAsync();
                var match = Regex.Match(pageContent, "//static.lostfilm.tv/Images/[\\d]+/Posters/t_shmoster_s[\\d]+.jpg");
                if (match == Match.Empty) return;
                var imageUri = match.Value;

                var imageRequest = new HttpRequestMessage(HttpMethod.Get, "http:" + imageUri);
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