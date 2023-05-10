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
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace backend.Pages
{
    public class KorisnikPageModel : PageModel
    {
        private readonly IDatabaseService _databaseService;
        private ISessionDataService _sessionDataService;
        private IHttpContextAccessor _httpContextAccessor;
        private ISession _session;
        private ICachingData _cachingData;
        private SessionData _sessionData;

        public KorisnikPageModel(IDatabaseService databaseService, ISessionDataService sessionDataService, IHttpContextAccessor httpContextAccessor)
        {
            _databaseService = databaseService;
            _sessionDataService = sessionDataService;
            _httpContextAccessor = httpContextAccessor;
            _session = _httpContextAccessor.HttpContext.Session;
            _sessionData = _sessionDataService.GetSessionData(_session);
            _cachingData = _sessionData.CachingData;
            btn = _cachingData.Btn;
            personalniTreners = _cachingData.PersonalniTreners;
            poruka = _cachingData.Poruka;
            _clan = _cachingData.Clans.FirstOrDefault();
        }

        public Clan _clan { get; set; }
        public PersonalniTrener _personalniTrener { get; set; }
        public List<PersonalniTrener> personalniTreners { get; set; }
        public int btn { get; set; }
        public string poruka { get; set; }
        [BindProperty]
        public int SelectedCalculation { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            _session = _httpContextAccessor.HttpContext.Session;
            _sessionData = _sessionDataService.GetSessionData(_session);
            _cachingData = _sessionData.CachingData;
            btn = _cachingData.Btn;
            personalniTreners = _cachingData.PersonalniTreners;
            poruka = _cachingData.Poruka;
            _clan = _cachingData.Clans.FirstOrDefault();

            try
            {   
               
                var user = await _sessionDataService.GetSessionDataAsync(_httpContextAccessor.HttpContext.Session);

                if (user.Id < 0)
                {
                    return RedirectToPage("/Prijava");
                }

                if (user.TipKorisnika == UserCategory.PersonalTrainer)
                {
                    return RedirectToPage("/Korisnik");
                }

                var resp = await _databaseService.GetClanove(new List<int> { user.Id });
                _cachingData.Clans.Add(resp.Data.First() as Clan);


            }
            catch (Exception ex)
            {
                RedirectToPage("/Prijava");
            }

            await _sessionDataService.SetSessionDataAsync(_session, _sessionData);
            return Page();
        }

        public async Task<IActionResult> OnPostPersonalne(CultureInfo id1)
        {
            try
            {
                _cachingData.Btn = 2;

                var resp = await _databaseService.GetPersonalneTrenere();

                if (resp != null && resp.Status)
                {
                    _cachingData.PersonalniTreners = resp.Data as List<PersonalniTrener>;
                    personalniTreners = _cachingData.PersonalniTreners;
                }
                if (!resp.Data.Any() || !_cachingData.PersonalniTreners.Any())
                {
                    _cachingData.Btn = 5;
                    _cachingData.Poruka = "Trenutno ne postoje treneri";
                }
            }
            catch (Exception ex)
            {
                RedirectToPage("/Korisnik");
            }
            await _sessionDataService.SetSessionDataAsync(_session,_sessionData);
            return RedirectToPage("/KorisnikPage");
        }

        public async Task<IActionResult> OnPostRequestpersonalni(int idz)
        {
            var requestCultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
            var requestCulture = requestCultureFeature.RequestCulture;
            var culture = requestCulture.Culture;
            try
            {
                if (idz>0)
                {
                    var vecPostoji = await _databaseService.GetPersonalneTrenereByClanID(_clan.ID);
                    if (vecPostoji.Status)
                    {
                        var p = vecPostoji.Data.FirstOrDefault() as PersonalniTrener;
                        if (p.ID == idz)
                        {
                            _cachingData.Btn = 4;
                            await _sessionDataService.SetSessionDataAsync(_session, _sessionData);
                            return Redirect("/KorisnikPage?culture=" + culture);
                        }
                    }
                    var resp = await _databaseService.RequestPersonalniTrener(idz,_clan.ID);
                    if (!resp.Status)
                    {
                        if (resp.Data != null)
                        {
                            _cachingData.Btn = -2;
                            await _sessionDataService.SetSessionDataAsync(_session, _sessionData);
                            return Redirect("/KorisnikPage?culture=" + culture);
                        }
                        _cachingData.Poruka = resp.Message.Single();
                        _cachingData.Btn = 5;
                        var pt = _cachingData.PersonalniTreners.Find(x => x.ID == idz);
                        _cachingData.PersonalniTreners.Remove(pt);
                        await _sessionDataService.SetSessionDataAsync(_session,_sessionData);
                        return Redirect("/KorisnikPage?culture=" + culture);
                    }

                    _cachingData.Poruka = resp.Message.Single();
                    var resp1 = await _databaseService.GetPersonalneTrenere(new List<int> { idz });
                    _cachingData.PersonalniTreners.Remove(resp1.Data.First() as PersonalniTrener);
                    personalniTreners = _cachingData.PersonalniTreners;
                }
    }
            catch(Exception ex)
            {
                _cachingData.Poruka = "Doslo je do greske...";
                _cachingData.Btn = 5;
                RedirectToPage("/KorisnikPage");
            }

            await _sessionDataService.SetSessionDataAsync(_session,_sessionData);
            return Redirect("/KorisnikPage?culture="+culture);
        }

        public async Task<IActionResult> OnPost()
        {
            if (SelectedCalculation != null)
            {
                if (SelectedCalculation == 0)
                {
                    if (personalniTreners.Count > 0)
                        _cachingData.PersonalniTreners = personalniTreners.OrderBy(x => x.Ime).ToList();
                }
                if (SelectedCalculation == 1)
                {
                    if (personalniTreners.Count > 0)
                        _cachingData.PersonalniTreners = personalniTreners.OrderBy(x => x.Prezime).ToList();
                }
            }

            var requestCultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
            var requestCulture = requestCultureFeature.RequestCulture;
            var culture = requestCulture.Culture;
            await _sessionDataService.SetSessionDataAsync(_session,_sessionData);
            return Redirect("/KorisnikPage?culture=" + culture);
        }
        
        public async Task<IActionResult> OnPostMorePT(CultureInfo id1)
        {
            _cachingData.Btn = 2;
            if (_cachingData.CPT == 10)
                _cachingData.CPT = 0;
            var res = await _databaseService.GetPersonalneTrenere(null, ++_cachingData.CPT);
            if (res.Status)
            {
                List<PersonalniTrener> lpt = new List<PersonalniTrener>();
                lpt = res.Data as List<PersonalniTrener>;
                _cachingData.PersonalniTreners= lpt;
            }
            await _sessionDataService.SetSessionDataAsync(_session,_sessionData);
            return Redirect("/KorisnikPage?culture=" + id1.Name);
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
