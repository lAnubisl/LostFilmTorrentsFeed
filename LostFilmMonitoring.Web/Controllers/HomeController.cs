using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LostFilmMonitoring.BLL.Models;
using LostFilmMonitoring.BLL.Implementations;

namespace LostFilmMonitoring.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly FeedService _feedService = new FeedService(new ConfigurationService());
        private readonly PresentationService _presentationService = new PresentationService(new ConfigurationService());

        [HttpGet, Route("{guid=}")]
        public async Task<ActionResult> Index(string guid)
        {
            var userId = Guid.Empty;
            if (!string.IsNullOrEmpty(guid) && !Guid.TryParse(guid, out userId))
            {
                return new NotFoundResult();
            }

            return View(await _presentationService.GetRegistrationViewModel(userId));
        }

        [HttpPost, Route("")]
        public async Task<ActionResult> Index(RegistrationViewModel model)
        {
            var userid = await _presentationService.Register(model);
            return RedirectToAction("Index", "Home", new { guid = userid.ToString() });
        }

        [HttpGet, Route("/rss/{userId}")]
        public async Task<IActionResult> Rss(Guid userId)
        {
            var stream = await _feedService.GetRss(userId);
            if (stream == null) return new NotFoundResult();
            return new FileStreamResult(stream, "application/rss+xml");
        }
    }
}