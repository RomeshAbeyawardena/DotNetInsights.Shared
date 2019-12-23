using System;
using DotNetInsights.Shared.Library;
using DotNetInsights.Shared.Library.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetInsights.Shared.Services.Attributes
{
    public sealed class RoleAuthoriseAttribute : Attribute, IAuthorizationFilter
    {
        private HttpContext GetHttpContext(AuthorizationFilterContext context) => context.HttpContext;
        private TService GetService<TService>(HttpContext context) => context.RequestServices.GetRequiredService<TService>();

        private IActionResult UnauthorisedResult(object value) => new UnauthorizedObjectResult(value);

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            ExceptionHandler.Try(() =>
            {
                var httpContext = GetHttpContext(context);
            
            var options = GetService<RoleAuthorisationOptions>(httpContext);

            httpContext.Request.Cookies.TryGetValue(options.CookieName, out var cookieString);

            var encryptedToken = Convert.FromBase64String(cookieString);

            if(encryptedToken.Length < 0)
                context.Result = UnauthorisedResult("EXCEPTION");

            });
            

        }
    }
}