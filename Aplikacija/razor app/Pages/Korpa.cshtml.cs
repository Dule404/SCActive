using backend.Models.UserAuth;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Threading.Tasks;
using backend.Models;
using backend.Models.DbResponse;

namespace backend.Pages
{
    public class Korpa : PageModel
    {
        private readonly ResourceManager _resourceManager;
        private readonly ISessionDataService _sessionDataService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDateTimeService _dateTimeService;
        private readonly IDatabaseService _databaseService;
        private readonly ITranslatorService _translatorService;
        private SessionData _userSessionData;

        public Korpa(IDatabaseService databaseService, IHttpContextAccessor httpContextAccessor, ISessionDataService sessionDataService, IDateTimeService dateTimeService, ITranslatorService translatorService)
        {
            _databaseService = databaseService;
            _httpContextAccessor = httpContextAccessor;
            _sessionDataService = sessionDataService;
            _dateTimeService = dateTimeService;
            _translatorService = translatorService;
            _resourceManager = new ResourceManager(typeof(Translations.korpa));
            _userSessionData = _sessionDataService.GetSessionData(_httpContextAccessor.HttpContext.Session);
        }

        public async Task OnGet()
        {
            _userSessionData = await _sessionDataService.GetSessionDataAsync(_httpContextAccessor.HttpContext.Session);
        }

        public async Task<IActionResult> OnPostRemoveFromCart(int index)
        {
            _userSessionData = await _sessionDataService.GetSessionDataAsync(_httpContextAccessor.HttpContext.Session);
            _userSessionData.Korpa.RemoveAt(index);
            await _sessionDataService.SetSessionDataAsync(_httpContextAccessor.HttpContext.Session, _userSessionData);
            return Page();
        }

            public async Task<IActionResult> OnPost()
        {
            string brTelefona = Request.Form["brTelefona"].ToString();
            string narucilac= Request.Form["ime"].ToString() + " " + Request.Form["prezime"].ToString();
            string adresa = Request.Form["ulica"].ToString() + ", " + Request.Form["grad"].ToString() + " " + Request.Form["pbroj"].ToString();
            _userSessionData = await _sessionDataService.GetSessionDataAsync(_httpContextAccessor.HttpContext.Session);
            var porudzbina = new Porudzbina();
            porudzbina.DatumPorudzbine = _dateTimeService.GetDateTimeNow();
            porudzbina.IDKorisnika = _userSessionData.Id;
            porudzbina.Narucilac = narucilac;
            porudzbina.Adresa = adresa;
            porudzbina.BrojTelefona = brTelefona;
            porudzbina.Proizvodi = new List<Models.Proizvod>();
            foreach (Models.Proizvod p in _userSessionData.Korpa)
            {
                DbResponse resp = _databaseService.ChangeQuantity(p.ID, p.Kolicina).Result;
                p.ID = 0;
            }
            var response = await _databaseService.PostPorudzbina(porudzbina);
                if(response.Status)
                {

                    _userSessionData.Korpa = new List<Models.Proizvod>();
                    await _sessionDataService.SetSessionDataAsync(_httpContextAccessor.HttpContext.Session, _userSessionData);
                    return RedirectToPage("/Prodavnica");
                }
                return Page();
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