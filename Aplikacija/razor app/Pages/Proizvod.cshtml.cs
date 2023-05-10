using System;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using backend.Models.DbResponse;
using backend.Models.UserAuth;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace backend.Pages
{
    public class Proizvod : PageModel
    {
        private readonly ISessionDataService _sessionDataService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDatabaseService _databaseService;
        private readonly ITranslatorService _translatorService;
        private ICachingData _cachingData;
        private SessionData _userSessionData;
        [BindProperty]
        public Models.Proizvod Product { get; set; }

        public Proizvod(IDatabaseService databaseService, IHttpContextAccessor httpContextAccessor, ISessionDataService sessionDataService, ITranslatorService translatorService)
        {
            _databaseService = databaseService;
            _httpContextAccessor = httpContextAccessor;
            _sessionDataService = sessionDataService;
            _translatorService = translatorService;
            _cachingData = _sessionDataService.GetSessionData(_httpContextAccessor.HttpContext.Session).CachingData;
        }

        public async Task<IActionResult> OnGet()
        {
            _cachingData = _sessionDataService.GetSessionData(_httpContextAccessor.HttpContext.Session).CachingData;
            int id = _cachingData.proizvodID;
            var resp = await _databaseService.GetProizvod(id);
            if (resp.Status)
                Product = (resp.Data.FirstOrDefault() as Models.Proizvod);
            else
            {
                Product = new Models.Proizvod();
                return RedirectToPage("/Prodavnica");
            }
            _userSessionData = await _sessionDataService.GetSessionDataAsync(_httpContextAccessor.HttpContext.Session);
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            _userSessionData = await _sessionDataService.GetSessionDataAsync(_httpContextAccessor.HttpContext.Session);
            var velicina = Request.Form["Velicina"];
            var kolicina = Request.Form["Kolicina"];
            _userSessionData.UkupnaKolicina.Add(Product.Kolicina);
            Product.Velicina = velicina;
            Product.Kolicina = Int32.Parse(kolicina);
            _userSessionData.Korpa.Add(Product);
            await _sessionDataService.SetSessionDataAsync(_httpContextAccessor.HttpContext.Session, _userSessionData);
            return RedirectToPage("/Prodavnica");
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