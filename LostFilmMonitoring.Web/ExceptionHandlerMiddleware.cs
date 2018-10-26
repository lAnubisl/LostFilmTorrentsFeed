using LostFilmMonitoring.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace LostFilmMonitoring.Web
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger logger)
        {
            _next = next;
            _logger = logger.CreateScope(nameof(ExceptionHandlerMiddleware));
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
            }
        }
    }

    public static class ExceptionHandlerMiddlewareExtensions
    {
        //public static IApplicationBuilder UseExceptionHandler(this IApplicationBuilder builder, ILogger logger)
        //{
        //    return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        //}
    }
}