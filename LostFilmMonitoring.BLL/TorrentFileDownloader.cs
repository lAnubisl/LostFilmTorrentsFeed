using LostFilmMonitoring.Common;
using LostFilmMonitoring.DAO.DAO;
using LostFilmMonitoring.DAO.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace LostFilmMonitoring.BLL
{
    internal sealed class TorrentFileDownloader
    {
        private readonly TorrentFileDAO _torrentFileDAO;
        private readonly ILogger _logger;

        internal TorrentFileDownloader(TorrentFileDAO torrentFileDAO, ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            _logger = logger.CreateScope(nameof(logger));
            _torrentFileDAO = torrentFileDAO ?? throw new ArgumentNullException(nameof(torrentFileDAO));
        }

        internal async Task<TorrentFile> Download(User user, int torrentFileId)
        {
            var cachedTorrent = _torrentFileDAO.TryFind(torrentFileId);
            if (cachedTorrent != null)
            {
                return cachedTorrent;
            }

            await DownloadInternal(user, torrentFileId);

            cachedTorrent = _torrentFileDAO.TryFind(torrentFileId);
            if (cachedTorrent == null)
            {
                _logger.Error("Cannot download torrent file");
            }

            return cachedTorrent;
        }

        private async Task DownloadInternal(User user, int torrentFileId)
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"http://n.tracktor.site/rssdownloader.php?id={torrentFileId}");
                request.Headers.Add("Cookie", $"uid={user.Uid};usess={user.Usess}");
                HttpResponseMessage response = null;

                try
                {
                    response = await client.SendAsync(request);
                }
                catch (Exception ex)
                {
                    _logger.Log(ex);
                    return;
                }

                if (response.Content.Headers.ContentType.MediaType != "application/x-bittorrent")
                {
                    string responseBody = null;
                    if (response.Content.Headers.ContentType.MediaType == "text/html")
                    {
                        responseBody = await response.Content.ReadAsStringAsync();
                    }

                    _logger.Error($"contentType is not 'application/x-bittorrent' it is '{response.Content.Headers.ContentType.MediaType}'. Response content is: '{responseBody}'.");
                    return;
                }

                var torrentFile = new TorrentFile();
                response.Content.Headers.TryGetValues("Content-Disposition", out IEnumerable<string> cd);
                torrentFile.FileName = cd?.FirstOrDefault()?.Substring("attachment;filename=\"".Length + 1);
                if (torrentFile.FileName == null)
                {
                    _logger.Error($"Something wrong with 'Content-Disposition' header of the response.");
                    return;
                }

                torrentFile.FileName = torrentFile.FileName.Substring(0, torrentFile.FileName.Length - 1);
                torrentFile.Stream = await response.Content.ReadAsStreamAsync();
                await _torrentFileDAO.Save(torrentFile, torrentFileId);
            }
        }
    }
}