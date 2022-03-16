using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Middleware
{
    public class UnlistenHttpMiddleware
    {
        private readonly RequestDelegate _next;

        public UnlistenHttpMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            bool redirected = false;
            if (!httpContext.Request.IsHttps && !httpContext.Request.Path.ToString().Contains("InvalidProtocol"))
            {
                httpContext.Response.Redirect($"{httpContext.Request.PathBase}/InvalidProtocol", false);
                redirected = true;
            }

            if (!redirected)
                await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class UnlistenHttpMiddlewareExtensions
    {
        /// <summary>
        /// Enforces https by having any call made through http be redirected to an error page.
        /// To see the reasonining behind this middleware, see the comments for UseHttpsRedirection and UseHsts middleware in the Configure method in Startup.cs
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseUnlistenHttpMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UnlistenHttpMiddleware>();
        }
    }
}
