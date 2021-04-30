using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LostFilmMonitoring.BLL.Interfaces;
using LostFilmMonitoring.BLL.Models;
using Newtonsoft.Json;
using LostFilmMonitoring.Web.Models;
using LostFilmMonitoring.Common;
using LostFilmMonitoring.BLL;

namespace LostFilmMonitoring.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly PresentationService _presentationService;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly ILogger _logger;

        public HomeController(PresentationService presentationService, ICurrentUserProvider currentUserProvider, ILogger logger)
        {
            _presentationService = presentationService;
            _currentUserProvider = currentUserProvider;
            _logger = logger.CreateScope(nameof(HomeController));
        }

        [HttpGet, Route("")]
        public async Task<ActionResult> Index() => View(await _presentationService.GetIndexModelAsync());

        [HttpPost, Route("")]
        public async Task<EmptyResult> Index(UpdateSubscriptionModel model)
        {
            await _presentationService.UpdateSubscriptionsAsync(model?.SelectedItems);
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
        public ViewResult Register() => View(new RegistrationModel());

        [HttpPost, Route("Register")]
        public async Task<ActionResult> Register(RegistrationModel model)
        {
            await _presentationService.RegisterAsync(model);
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
                    await _presentationService.UpdateSubscriptionsAsync(selected);
                }

                Response.Cookies.Delete("selected");
            }

            return RedirectToAction("Feed");
        }

        [HttpGet, Route("About")]
        public ViewResult About() => View();


        [HttpGet, Route("Instructions")]
        public ViewResult Instructions() => View();
    }
}
