using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using backend.Enums;
using backend.Models;
using backend.Models.UserAuth;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Localization;

namespace backend.Pages
{
    public class Korisnik : PageModel
    {
        private readonly IDatabaseService _databaseService;
        private ISessionDataService _sessionDataService;
        private IHttpContextAccessor _httpContextAccessor;
        private ISession _session;
        private SessionData _sessionData;
        public ICachingData CachingData;

        public Clan clan { get; set; }
        public PersonalniTrener personalniTrener { get; set; }
        public Administrator admin { get; set; }
        public UserCategory UserCategory { get; set; }
        public string Slika { get; set; }
        public int Btn { get; set; }

        public Korisnik(IDatabaseService databaseService, ISessionDataService sessionDataService, IHttpContextAccessor httpContextAccessor)
        {
            _databaseService = databaseService;
            _sessionDataService = sessionDataService;
            _httpContextAccessor = httpContextAccessor;
            _httpContextAccessor = httpContextAccessor;
            _session = _httpContextAccessor.HttpContext.Session;
            _sessionData = _sessionDataService.GetSessionData(_session);
            CachingData = _sessionData.CachingData;
            Btn = CachingData.Btn;
        }
        public async Task<ActionResult> OnGet()
        {
            try
            {
                var user = await _sessionDataService.GetSessionDataAsync(_httpContextAccessor.HttpContext.Session);
                
                if (user.Id < 0)
                {
                    return RedirectToPage("/Prijava");
                }

                UserCategory = user.TipKorisnika;
                Btn = CachingData.Btn;

                if (UserCategory == UserCategory.DefaultUser)
                {
                    var resp = await _databaseService.GetClanove(new List<int> { user.Id });
                    if (resp.Status)
                    {
                        clan = new Clan();
                        clan = resp.Data.First() as Clan;
                        if(clan.Slika!=null)
                            Slika = clan.Slika;
                    }
                    else
                    {
                        RedirectToPage("/Prijava");
                    }
                    var resp2 = await _databaseService.GetPersonalneTrenereByClanID(clan.ID);
                    if(resp2.Status)
                    {
                        personalniTrener=resp2.Data.First() as PersonalniTrener;
                    }
                }
                if (UserCategory == UserCategory.PersonalTrainer)
                {
                    var resp2 = await _databaseService.GetPersonalneTrenere(new List<int> { user.Id });
                    if (resp2.Status)
                    {
                        personalniTrener=new PersonalniTrener();
                        personalniTrener = resp2.Data.First() as PersonalniTrener;
                        if (personalniTrener != null && personalniTrener.Slika != null)
                            Slika = personalniTrener.Slika;
                    }
                }
                if (UserCategory == UserCategory.Administrator)
                {
                    var resp2 = await _databaseService.GetAdministrator(new List<int> { user.Id });
                    if (resp2.Status)
                    {
                        admin = resp2.Data.First() as Administrator;
                        if (admin != null && admin.Slika != null)
                            Slika = admin.Slika;
                    }
                }

            }
            catch(Exception e)
            {
                RedirectToPage("/Prijava");
            }
            
            return Page();
        }

        public async Task<IActionResult> OnPostObrisi(int idt)
        {
            var requestCultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
            var requestCulture = requestCultureFeature.RequestCulture;
            var culture = requestCulture.Culture;
            var user = await _sessionDataService.GetSessionDataAsync(_httpContextAccessor.HttpContext.Session);

            try
            {
                var resp = await _databaseService.PersonalniTrenerObrisiClana(idt, user.Id);
                if (!resp.Status)
                {
                    CachingData.Btn = -1;
                }
            }
            catch(Exception e)
            {
                return Redirect("/Korisnik?culture=" + culture);
            }
            await _sessionDataService.SetSessionDataAsync(_session, _sessionData);
            return Redirect("/Korisnik?culture=" + culture);
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