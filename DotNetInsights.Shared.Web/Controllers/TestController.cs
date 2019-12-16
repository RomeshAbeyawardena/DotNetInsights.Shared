using Microsoft.AspNetCore.Mvc;
using DotNetInsights.Shared.Contracts;
using DotNetInsights.Shared.Services;
using DotNetInsights.Shared.Services.Extensions;
using DotNetInsights.Shared.WebApp.Handlers;
using System.Threading.Tasks;
using DotNetInsights.Shared.Domains.Enumerations;
using DotNetInsights.Shared.Services.Exceptions;

namespace DotNetInsights.Shared.WebApp.Controllers
{
    [Route("{controller}/{action}")]
    public class TestController : Controller
    {
        public TestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<ActionResult> Test([FromQuery]Test test)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            await _mediator.Push(new Test()).ConfigureAwait(false);

            await _mediator.NotifyAsync(DefaultEntityChangedEvent.Create(test, entityEventType: EntityEventType.Added)).ConfigureAwait(false);
            return Ok(test);
        }

        public ActionResult Index()
        {
            return View();
        }

        private readonly IMediator _mediator;
    }
}