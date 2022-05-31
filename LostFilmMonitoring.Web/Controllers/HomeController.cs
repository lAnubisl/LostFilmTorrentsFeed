// <copyright file="HomeController.cs" company="Alexander Panfilenok">
// MIT License
// Copyright (c) 2021 Alexander Panfilenok
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the 'Software'), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// </copyright>

namespace LostFilmMonitoring.Web.Controllers
{
    using System;
    using System.Threading.Tasks;
    using LostFilmMonitoring.BLL;
    using LostFilmMonitoring.BLL.Models;
    using LostFilmMonitoring.Web.Models;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    /// <summary>
    /// HomeController.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly PresentationService presentationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="presentationService">PresentationService.</param>
        public HomeController(PresentationService presentationService)
        {
            this.presentationService = presentationService;
        }

        /// <summary>
        /// Index.
        /// </summary>
        /// <returns>ActionResult.</returns>
        [HttpGet]
        [Route("")]
        public async Task<ActionResult> Index() => this.View(await this.presentationService.GetIndexModelAsync());

        /// <summary>
        /// Index.
        /// </summary>
        /// <param name="model">UpdateSubscriptionModel.</param>
        /// <returns>EmptyResult.</returns>
        [HttpPost]
        [Route("")]
        public async Task<EmptyResult> Index(UpdateSubscriptionModel model)
        {
            await this.presentationService.UpdateSubscriptionsAsync(model?.SelectedItems);
            return new EmptyResult();
        }

        /// <summary>
        /// Login.
        /// </summary>
        /// <returns>ViewResult.</returns>
        [HttpGet]
        [Route("Login")]
        public ViewResult Login() => this.View();

        /// <summary>
        /// Login.
        /// </summary>
        /// <param name="userId">UserId.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(Guid userId)
        {
            var success = await this.presentationService.Authenticate(userId);
            if (!success)
            {
                this.ModelState.AddModelError("error", "К сожалению, мы не нашли пользователя с таким ключем. Либо у вас не верный ключ, либо пользователь с таким ключем был удалён из-за длительного отсутствия активности.");
                return this.View();
            }

            return this.RedirectToAction("Index");
        }

        /// <summary>
        /// Register.
        /// </summary>
        /// <returns>ViewResult.</returns>
        [HttpGet]
        [Route("Register")]
        public ViewResult Register() => this.View(new EditUserRequestModel());

        /// <summary>
        /// Register.
        /// </summary>
        /// <param name="model">RegistrationModel.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        [Route("Register")]
        public async Task<ActionResult> Register(EditUserRequestModel model)
        {
            await this.presentationService.RegisterAsync(model);
            if (this.Request.Cookies.ContainsKey("selected"))
            {
                var selectedJson = this.Request.Cookies["selected"];
                SubscriptionItem[] selected = null;
                try
                {
                    selected = JsonConvert.DeserializeObject<SubscriptionItem[]>(selectedJson);
                }
                catch
                {
                    // DO NOTHING
                }

                if (selected != null)
                {
                    await this.presentationService.UpdateSubscriptionsAsync(selected);
                }

                this.Response.Cookies.Delete("selected");
            }

            return this.RedirectToAction("Feed", "Feed");
        }

        /// <summary>
        /// About.
        /// </summary>
        /// <returns>ViewResult.</returns>
        [HttpGet]
        [Route("About")]
        public ViewResult About() => this.View();

        /// <summary>
        /// Uid.
        /// </summary>
        /// <returns>ViewResult.</returns>
        [HttpGet]
        [Route("Uid")]
        public ViewResult Uid() => this.View();

        /// <summary>
        /// Usess.
        /// </summary>
        /// <returns>ViewResult.</returns>
        [HttpGet]
        [Route("Usess")]
        public ViewResult Usess() => this.View();

        /// <summary>
        /// IfSession.
        /// </summary>
        /// <returns>ViewResult.</returns>
        [HttpGet]
        [Route("IfSession")]
        public ViewResult IfSession() => this.View();

        /// <summary>
        /// TrackerId.
        /// </summary>
        /// <returns>ViewResult.</returns>
        [HttpGet]
        [Route("TrackerId")]
        public ViewResult TrackerId() => this.View();
    }
}
