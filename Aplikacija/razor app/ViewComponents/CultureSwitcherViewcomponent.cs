using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;
using backend.Models;
using backend.Models.CultureModel;
using backend.Models.UserAuth;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace backend.ViewComponents
{
    public class CultureSwitcherViewComponent : ViewComponent
    {
        private readonly IOptions<RequestLocalizationOptions> _localizationOptions;
        private readonly ISessionDataService _sessionDataService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CultureSwitcherViewComponent(IOptions<RequestLocalizationOptions> localizationOptions,ISessionDataService sessionDataService,IHttpContextAccessor httpContextAccessor)
        {
            this._localizationOptions = localizationOptions;
            _sessionDataService = sessionDataService;
            _httpContextAccessor = httpContextAccessor;
        }

        public IViewComponentResult Invoke()
        {
            
            var user = _sessionDataService.GetSessionData(_httpContextAccessor.HttpContext.Session);
            var cultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
            
            var model = new CultureSwitcherModel
            {
                SupportedCultures = _localizationOptions.Value.SupportedUICultures.ToList(),
                CurrentUICulture = cultureFeature.RequestCulture.UICulture
            };
            user.CachingData.currentCulture = cultureFeature.RequestCulture.UICulture;
            _sessionDataService.SetSessionData(_httpContextAccessor.HttpContext.Session,user);
            return View(model);
        }
    }
}