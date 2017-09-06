using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace NoCacheMiddleware.Middlewares
{
    public class DisableCacheMiddleware
    {
        private readonly RequestDelegate _next;

        public DisableCacheMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            Console.WriteLine($"Request for {context.Request.Path} received ({context.Request.ContentLength ?? 0} bytes)");

            context.Response.OnStarting(state =>
            {
                var httpContext = (HttpContext)state;
                httpContext.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
                httpContext.Response.Headers.Add("Pragma", "no-cache");
                httpContext.Response.Headers.Add("Expires", "0");
                return Task.FromResult(0);
            }, context);

            await _next.Invoke(context);
        }
    }

    public static class DisableMiddlewareExtensions
    {
        public static IApplicationBuilder UseDisableCacheMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DisableCacheMiddleware>();
        }
    }
}
