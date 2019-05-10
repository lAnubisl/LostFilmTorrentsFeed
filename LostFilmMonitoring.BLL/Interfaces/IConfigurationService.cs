namespace LostFilmMonitoring.BLL.Interfaces
{
    public interface IConfigurationService
    {
        string GetConnectionString();

        string GetBasePath();

        string BaseFeedCookie();

        string GetImagesDirectory();

        string GetTorrentCachePath();
    }
}