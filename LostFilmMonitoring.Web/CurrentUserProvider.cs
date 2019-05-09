using LostFilmMonitoring.BLL.Interfaces;
using Microsoft.AspNetCore.Http;
using System;

namespace LostFilmMonitoring.Web
{
    public class CurrentUserProvider : ICurrentUserProvider
    {
        private static string Key = "UserId";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private Guid _userId;

        public CurrentUserProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid GetCurrentUserId()
        {
            if (_userId != Guid.Empty) return _userId;
            Guid.TryParse(_httpContextAccessor.HttpContext.Request.Cookies[Key], out _userId);
            return _userId;
        }

        public void SetCurrentUserId(Guid userId)
        {
            _userId = userId;
            _httpContextAccessor.HttpContext.Response.Cookies.Append(
                Key,
                userId.ToString(),
                new CookieOptions()
                {
                    Expires = DateTimeOffset.Now.AddYears(userId != Guid.Empty ? 1 : -1),
                    HttpOnly = true,
                }
            );
        }
    }
}