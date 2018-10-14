using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LostFilmMonitoring.BLL.Implementations;
using LostFilmMonitoring.BLL.Interfaces;
using System.IO;
using LostFilmMonitoring.BLL.Models;

namespace LostFilmMonitoring.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IFeedService _feedService;
        private readonly IPresentationService _presentationService;
        private readonly ICurrentUserProvider _currentUserProvider;

        public HomeController(IFeedService feedService, IPresentationService presentationService, ICurrentUserProvider currentUserProvider)
        {
            _feedService = feedService;
            _presentationService = presentationService;
            _currentUserProvider = currentUserProvider;
        }

        private readonly ILostFilmRegistrationService registrationService = new LostFilmRegistrationService();

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
        public async Task<RedirectToActionResult> Register(string captcha)
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
            }

            return RedirectToAction("Login");
        }

        [HttpGet, Route("Captcha")]
        public async Task<FileStreamResult> Captcha()
        {
            var captcha = await registrationService.GetNewCaptcha();
            Response.Cookies.Append("captcha", captcha.SessionKey);
            return new FileStreamResult(new MemoryStream(captcha.CaptchaGif), "image/gif");
        }

        [HttpGet, Route("About")]
        public ViewResult About() => View();

        [HttpGet, Route("Online")]
        public async Task<IActionResult> Online()
        {
            var items = await _feedService.GetItems();
            if (items == null) return new NotFoundResult();
            return View(items);
        }

        [HttpGet, Route("Feed")]
        public async Task<IActionResult> Feed() => View(_currentUserProvider.GetCurrentUserId());

        [HttpGet, Route("Rss/{userId}")]
        public async Task<IActionResult> Rss(Guid userId)
        {
            var stream = await _feedService.GetRss(userId);
            if (stream == null) return new NotFoundResult();
            return new FileStreamResult(stream, "application/rss+xml");
        }

        [HttpGet, Route("Instructions")]
        public ViewResult Instructions() => View();
    }

    public class UpdateSubscriptionModel
    {
        public SelectedFeedItem[] SelectedItems { get; set; }
    }
}