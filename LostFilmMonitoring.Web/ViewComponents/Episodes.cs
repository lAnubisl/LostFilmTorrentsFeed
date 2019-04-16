using LostFilmMonitoring.BLL.Models;

namespace LostFilmMonitoring.Web.ViewComponents
{
    public class Episodes
    {
        public Episodes(string title, SerialViewModel[] items)
        {
            Title = title;
            Items = items;
        }

        public SerialViewModel[] Items { get; }
        public string Title { get; }
    }
}