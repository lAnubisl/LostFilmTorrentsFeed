using LostFilmMonitoring.Common;
using LostFilmMonitoring.DAO.Interfaces;
using LostFilmMonitoring.DAO.Interfaces.DomainModels;

namespace LostFilmMonitoring.DAO.Azure
{
    public class AzureBlobStorageTorrentFileDAO : ITorrentFileDAO
    {
        private readonly AzureBlobStorageClient azureBlobStorageClient;
        private readonly ILogger logger;

        public AzureBlobStorageTorrentFileDAO(AzureBlobStorageClient azureBlobStorageClient, ILogger logger)
        {
            this.azureBlobStorageClient = azureBlobStorageClient;
            this.logger = logger.CreateScope(nameof(AzureBlobStorageTorrentFileDAO));
        }

        public Task SaveAsync(string fileName, Stream fileContentStream, int torrentId)
        {
            this.logger.Info($"Call: {nameof(SaveAsync)}('{fileName}', Stream, {torrentId})");
            return this.azureBlobStorageClient.UploadAsync("torrentfiles", $"{torrentId}_{fileName}", fileContentStream);
        }

        public TorrentFile TryFind(int torrentId)
        {
            throw new NotImplementedException();
        }
    }
}
