using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LostFilmMonitoring.BLL.Models;
using LostFilmMonitoring.BLL.Implementations;
using Microsoft.AspNetCore.Http;

namespace LostFilmMonitoring.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly FeedService _feedService = new FeedService(new ConfigurationService());
        private readonly PresentationService _presentationService = new PresentationService(new ConfigurationService());

        [HttpGet, Route("{guid=}")]
        public async Task<ActionResult> Index(string guid)
        {
            if (string.IsNullOrEmpty(guid) && Request.Cookies.ContainsKey("userId"))
            {
                return new RedirectToActionResult("Index", "Home", new { guid = Request.Cookies["userId"] });
            }
            var userId = Guid.Empty;
            if (!string.IsNullOrEmpty(guid) && !Guid.TryParse(guid, out userId))
            {
                return new NotFoundResult();
            }

            return View(await _presentationService.GetRegistrationViewModel(userId));
        }

        [HttpPost, Route("")]
        public async Task<JsonResult> Index(RegistrationViewModel model)
        {
            var userId = await _presentationService.Register(model);
            if (userId == Guid.Empty) return new JsonResult(string.Empty);
            Response.Cookies.Append(
                "userId",
                userId.ToString(),
                new CookieOptions() { HttpOnly = true, Expires = DateTime.UtcNow.AddYears(1) });
            return new JsonResult(userId);
        }

        [HttpGet, Route("About")]
        public ViewResult About()
        {
            return View();
        }

        [HttpGet, Route("/online/{userId}")]
        public async Task<IActionResult> Online(Guid userId)
        {
            var items = await _feedService.GetItems(userId);
            if (items == null) return new NotFoundResult();
            return View(items);
        }

        [HttpGet, Route("/rss/{userId}")]
        public async Task<IActionResult> Rss(Guid userId)
        {
            var stream = await _feedService.GetRss(userId);
            if (stream == null) return new NotFoundResult();
            return new FileStreamResult(stream, "application/rss+xml");
        }

        [HttpGet, Route("/instructions")]
        public ViewResult Instructions()
        {
            return View();
        }
    }
}