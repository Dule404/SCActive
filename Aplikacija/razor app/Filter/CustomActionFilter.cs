using backend.Enums;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace backend.Filter
{
    public class CustomActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var _sessionDataService = context.HttpContext.RequestServices.GetService(typeof(ISessionDataService)) as ISessionDataService;
            var user = _sessionDataService.GetSessionData(context.HttpContext.Session);

            if (user.TipKorisnika != UserCategory.Administrator)
            {
                context.Result = new Microsoft.AspNetCore.Mvc.StatusCodeResult((int)HttpStatusCode.NotFound);
                base.OnActionExecuting(context);
            }
        }
    }
}
