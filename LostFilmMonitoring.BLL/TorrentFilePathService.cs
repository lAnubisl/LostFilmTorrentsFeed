using LostFilmMonitoring.Common;
using System;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LostFilmMonitoring.BLL
{
    public class TorrentFilePathService
    {
        private readonly ILogger logger;
        public TorrentFilePathService(ILogger logger)
        {
            this.logger = logger.CreateScope(nameof(TorrentFilePathService));
        }

        public async Task<string> GetTorrentLink(string feedLink, string cookie, string quantity)
        {
            using (var httpClient = new HttpClient())
            {
                var episodeRequest = new HttpRequestMessage(HttpMethod.Get, feedLink.Replace("lostfilm.tv", "www.lostfilm.tv"));
                episodeRequest.Headers.Add("cookie", "lf_session=" + cookie);
                var episodeResponse = await httpClient.SendAsync(episodeRequest);
                var episodeContent = await episodeResponse.Content.ReadAsStringAsync();
                var episodeIdMatch = Regex.Match(episodeContent, "PlayEpisode\\('(\\d+)'\\)");
                if (!episodeIdMatch.Success)
                {
                    logger.Error($"Cannot get episodeId from: {Environment.NewLine}{episodeContent}");
                    return null;
                }

                var episodeId = episodeIdMatch.Groups[1].Value;
                var linkRequest = new HttpRequestMessage(HttpMethod.Get, $"https://www.lostfilm.tv/v_search.php?a={episodeId}");
                linkRequest.Headers.Add("cookie", "lf_session=" + cookie);
                var linkResponse = await httpClient.SendAsync(linkRequest);
                var linkResponseBytes = await linkResponse.Content.ReadAsByteArrayAsync();
                var linkResponseContent = Encoding.UTF8.GetString(linkResponseBytes);
                var trackerLinkMatch = Regex.Match(linkResponseContent, "location.replace\\(\"([^\"]+)\"\\);");
                if (!trackerLinkMatch.Success)
                {
                    logger.Error($"Cannot get tracker link from: {Environment.NewLine}{linkResponseContent}");
                    return null;
                }

                var trackerLink = trackerLinkMatch.Groups[1].Value;
                var torrentFileLintsRequest = new HttpRequestMessage(HttpMethod.Get, trackerLink);
                var torrentFilesLinkResponse = httpClient.SendAsync(torrentFileLintsRequest).Result;
                var torrentFilesLinkContent = torrentFilesLinkResponse.Content.ReadAsStringAsync().Result;
                var torrentLinksMatches = Regex.Matches(torrentFilesLinkContent, "\\>http:\\/\\/tracktor\\.site\\/td\\.php\\?s=([^<]+)\\<");
                var torrentQualityMatches = Regex.Matches(torrentFilesLinkContent, "<div class=\"inner-box--label\">([^<]+)");
                for (int i = 0; i < torrentQualityMatches.Count; i++)
                {
                    if (torrentQualityMatches[i].Groups[1].Value.Trim().Equals(quantity))
                    {
                        return "http://tracktor.in/td.php?s=" + torrentLinksMatches[i].Groups[1].Value;
                    }
                }

                logger.Error($"Cannot get torrent link: {Environment.NewLine}{torrentFilesLinkContent}");
                return null;
            }
        }
    }
}