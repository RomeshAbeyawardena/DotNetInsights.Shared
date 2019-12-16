using Microsoft.AspNetCore.Mvc.Filters;
using DotNetInsights.Shared.Services.Exceptions;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DotNetInsights.Shared.Services.Middleware
{
    public class HandleModelStateErrorFilter : IAsyncExceptionFilter
    {
        public async Task OnExceptionAsync(ExceptionContext context)
        {
            if(context == null)
                throw new ArgumentNullException(nameof(context));

            await Task.CompletedTask.ConfigureAwait(false);

            if (context.Exception is ModelStateException modelStateException)
            {
                context.ExceptionHandled = true;
                foreach (var modelState in modelStateException.ModelStateBuilder)
                    context.ModelState.AddModelError(modelState.Key, modelState.Value);

                context.Result = new BadRequestObjectResult(context.ModelState);
            }
        }
    }
}
