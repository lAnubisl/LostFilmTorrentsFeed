using LostFilmMonitoring.Common;
using LostFilmMonitoring.DAO.DomainModels;
using System.IO;
using System.Threading.Tasks;

namespace LostFilmMonitoring.DAO.DAO
{
    public class TorrentFileDAO
    {
        private readonly string _basePath;
        private readonly ILogger _logger;

        public TorrentFileDAO(string basePath, ILogger logger)
        {
            _basePath = basePath;
            _logger = logger.CreateScope(nameof(TorrentFileDAO));
        }

        public async Task SaveAsync(string fileName, Stream content, int torrentId)
        {
            fileName = $"{torrentId}_{fileName}";
            using (var fs = new FileStream(
                Path.Combine(_basePath, fileName),
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 4096,
                useAsync: true)
            )
            {
                await content.CopyToAsync(fs);
            }

            _logger.Info($"File '{fileName}' downloaded.");
        }

        public TorrentFile TryFind(int torrentId)
        {
            var directory = new DirectoryInfo(_basePath);
            var files = directory.GetFiles($"{torrentId}_*.torrent");
            if (files.Length == 0)
            {
                return null;
            }

            return new TorrentFile
            {
                Stream = files[0].OpenRead(),
                FileName = files[0].Name
            };
        }
    }
}
