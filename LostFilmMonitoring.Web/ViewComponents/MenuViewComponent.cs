using LostFilmMonitoring.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;

namespace LostFilmMonitoring.Web.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly ICurrentUserProvider _currentUserProvider;

        public MenuViewComponent(ICurrentUserProvider currentUserProvider)
        {
            _currentUserProvider = currentUserProvider;
        }

        public IViewComponentResult Invoke()
        {
            var result = _currentUserProvider.GetCurrentUserId() != Guid.Empty;
            return View(result);
        }
    }
}