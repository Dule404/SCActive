using System.Globalization;
using System.Threading.Tasks;
using backend.Models.UserAuth;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace backend.Pages
{
    public class _LayoutLogged : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISessionDataService _sessionDataService;
        

        public _LayoutLogged(IHttpContextAccessor httpContextAccessor, ISessionDataService sessionDataService)
        {
            _httpContextAccessor = httpContextAccessor;
            _sessionDataService = sessionDataService;
        }

        public void OnGet()
        {
            
        }

        public RedirectResult OnPostProfile()
        {
            return Redirect("/Korisnik?culture=" + CultureInfo.CurrentCulture);
        }

        public async Task<IActionResult> OnPostLogOut()
        {
            var _userSessionData = new SessionData();
            await _sessionDataService.SetSessionDataAsync(_httpContextAccessor.HttpContext.Session, _userSessionData);
            return Redirect("/Index?culture=" + CultureInfo.CurrentCulture);
        }
    }
}