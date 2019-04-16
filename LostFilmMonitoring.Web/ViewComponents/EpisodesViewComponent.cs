using Microsoft.AspNetCore.Mvc;

namespace LostFilmMonitoring.Web.ViewComponents
{
    public class EpisodesViewComponent : ViewComponent
    {
        public EpisodesViewComponent()
        {
        }

        public IViewComponentResult Invoke(Episodes episodes)
        {
            return View(episodes);
        }
    }
}