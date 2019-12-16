using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace DotNetInsights.Shared.Services.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class BadRequestOnInvalidModelStateAttribute : Attribute, IActionFilter, IAsyncActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if(context == null)
                throw new ArgumentNullException(nameof(context));

            if(!context.ModelState.IsValid)
                context.Result = BadRequestActionResult(context.ModelState);
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if(context == null)
                throw new ArgumentNullException(nameof(context));

            if(next == null)
                throw new ArgumentNullException(nameof(context));

            if(context.ModelState.IsValid){
                await next().ConfigureAwait(false);
                return;
            }

            context.Result = BadRequestActionResult(context.ModelState);
        }

        private IActionResult BadRequestActionResult(ModelStateDictionary modelStateDictionary)
        {
            var badRequestActionResult = new BadRequestObjectResult(modelStateDictionary);

            return badRequestActionResult;
        }
    }
}
