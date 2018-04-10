using LostFilmMonitoring.DAO.DomainModels;

namespace LostFilmMonitoring.BLL
{
    internal static class Extensions
    {
        internal static string Serial(this FeedItem item)
        {
            return item.Title.Substring(0, item.Title.IndexOf(").") + 2);
        }
    }
}