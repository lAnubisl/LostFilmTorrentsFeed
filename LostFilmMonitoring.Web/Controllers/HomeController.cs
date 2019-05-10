using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LostFilmMonitoring.BLL.Implementations;
using LostFilmMonitoring.BLL.Interfaces;
using System.IO;
using LostFilmMonitoring.BLL.Models;
using Newtonsoft.Json;
using LostFilmMonitoring.Web.Models;
using LostFilmMonitoring.Common;

namespace LostFilmMonitoring.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IFeedService _feedService;
        private readonly IPresentationService _presentationService;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly ILogger _logger;
        private readonly ILostFilmRegistrationService _registrationService;

        public HomeController(IFeedService feedService, IPresentationService presentationService, ICurrentUserProvider currentUserProvider, ILogger logger)
        {
            _feedService = feedService;
            _presentationService = presentationService;
            _currentUserProvider = currentUserProvider;
            _logger = logger.CreateScope(nameof(HomeController));
            _registrationService = new LostFilmRegistrationService(_logger);
        }

        [HttpGet, Route("")]
        public async Task<ActionResult> Index() => View(await _presentationService.GetIndexModel());

        [HttpPost, Route("")]
        public async Task<EmptyResult> Index(UpdateSubscriptionModel model)
        {
            await _presentationService.UpdateSubscriptions(model?.SelectedItems);
            return new EmptyResult();
        }

        [HttpGet, Route("Login")]
        public ViewResult Login() => View();

        [HttpPost, Route("Login")]
        public async Task<IActionResult> Login(Guid userId)
        {
            var success = await _presentationService.Authenticate(userId);
            if (!success)
            {
                ModelState.AddModelError("error", "К сожалению, мы не нашли пользователя с таким ключем. Либо у вас не верный ключ, либо пользователь с таким ключем был удалён из-за длительного отсутствия активности.");
                return View();
            }

            return RedirectToAction("Index");
        }

        [HttpGet, Route("Register")]
        public ViewResult Register() => View(_currentUserProvider.GetCurrentUserId());

        [HttpPost, Route("Register")]
        public async Task<ActionResult> Register(string captcha)
        {
            var cookie = Request.Cookies["captcha"];
            if (cookie == null)
            {
                throw new Exception();
            }

            var result = await _presentationService.Register(captcha, cookie);
            if (!result.Success)
            {
                ModelState.AddModelError("error", result.Error);
                return View(Guid.Empty);
            }

            if (Request.Cookies.ContainsKey("selected"))
            {
                var selectedJson = Request.Cookies["selected"];
                SelectedFeedItem[] selected = null;
                try
                {
                    selected = JsonConvert.DeserializeObject<SelectedFeedItem[]>(selectedJson);
                }
                catch
                {
                    // DO NOTHING
                }

                if (selected != null)
                {
                    await _presentationService.UpdateSubscriptions(selected);
                }

                Response.Cookies.Delete("selected");
            }

            return RedirectToAction("Feed");
        }

        [HttpGet, Route("Captcha")]
        public async Task<FileStreamResult> Captcha()
        {
            var captcha = await _registrationService.GetNewCaptcha();
            Response.Cookies.Append("captcha", captcha.SessionKey);
            return new FileStreamResult(new MemoryStream(captcha.CaptchaGif), "image/gif");
        }

        [HttpGet, Route("About")]
        public ViewResult About() => View();

        [HttpGet, Route("Feed")]
        public async Task<IActionResult> Feed()
        {
            var model = await _feedService.GetFeedViewModel();
            if (model == null) return RedirectToAction("index");
            return View(model);
        }

        [HttpGet, Route("Rss/{userId}")]
        public async Task<IActionResult> Rss(Guid userId)
        {
            var stream = await _feedService.GetRss(userId);
            if (stream == null) return new NotFoundResult();
            return File(stream, "application/rss+xml", userId + ".xml");
        }

        [HttpGet, Route("Rss/{userId}/{id}")]
        public async Task<IActionResult> RssItem(int id, Guid userId)
        {
            var result = await _feedService.GetRssItem(userId, id);
            if (result == null) return new NotFoundResult();
            return File(result.Body, result.ContentType, result.FileName);
        }

        [HttpGet, Route("Instructions")]
        public ViewResult Instructions() => View();
    }
}