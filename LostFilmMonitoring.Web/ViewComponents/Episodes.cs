using LostFilmMonitoring.BLL.Models;

namespace LostFilmMonitoring.Web.ViewComponents
{
    public class Episodes
    {
        public Episodes(string title, SeriesViewModel[] items)
        {
            Title = title;
            Items = items;
        }

        public SeriesViewModel[] Items { get; }
        public string Title { get; }
    }
}