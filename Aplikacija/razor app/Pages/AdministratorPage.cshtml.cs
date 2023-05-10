using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using backend.Models;
using backend.Models.UserAuth;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace backend.Pages
{
    public class AdministratorPage : PageModel
    {
        private readonly ISessionDataService _sessionDataService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDatabaseService _databaseService;
        private readonly ITranslatorService _translatorService;
        private ISession _session;
        private ICachingData _cachingData;
        private SessionData _sessionData;
        
        public List<Clan> _clanovi { get; set; }
        public List<PersonalniTrener> _personalniTreneri { get; set; }
        public int btn { get; set; }
        [BindProperty]
        public int SelectedCalculation { get; set; }
        public AdministratorPage(ISessionDataService sessionDataService, IHttpContextAccessor httpContextAccessor, IDatabaseService databaseService, ITranslatorService translatorService)
        {
            _sessionDataService = sessionDataService;
            _httpContextAccessor = httpContextAccessor;
            _databaseService = databaseService;
            _translatorService = translatorService;
            _session = _httpContextAccessor.HttpContext.Session;
            _sessionData = _sessionDataService.GetSessionData(_session);
            _cachingData = _sessionData.CachingData;
            _clanovi = _cachingData.Clans;
            _personalniTreneri = _cachingData.PersonalniTreners;
            btn = _cachingData.Btn;
        }
        
        public void OnGet()
        {
            _session = _httpContextAccessor.HttpContext.Session;
            _sessionData = _sessionDataService.GetSessionData(_session);
            _cachingData = _sessionData.CachingData;
            _clanovi = _cachingData.Clans;
            _personalniTreneri = _cachingData.PersonalniTreners;
            btn = _cachingData.Btn;
        }

        public async Task<IActionResult> OnPostClanovi(CultureInfo id1)
        {
            _cachingData.Btn = 0;
            if (_cachingData.CC != 0)
                return Redirect("/AdministratorPage?culture=" +id1.Name);
            var res =  await _databaseService.GetClanove();
            if (res.Status)
            {
                foreach (var el in res.Data as List<Clan>)
                {
                    if (_cachingData.Clans.Find(x=>x.ID == el.ID) == null )
                    {
                        _cachingData.Clans.Add(el);
                    }
                }
            }
            await _sessionDataService.SetSessionDataAsync(_session,_sessionData);
            return Redirect("/AdministratorPage?culture=" +id1.Name);
        }

        public async Task<IActionResult> OnPostPersonalniTreneri(CultureInfo id1)
        {
            _cachingData.Btn = 2;
            if (_cachingData.CPT != 0)
                return Redirect("/AdministratorPage?culture=" +id1.Name);
            var res = await _databaseService.GetPersonalneTrenere();
            if (res.Status)
            {
                foreach (var el in res.Data as List<PersonalniTrener>)
                {
                    if (_cachingData.PersonalniTreners.Find(x=>x.ID == el.ID) == null)
                    {
                        _cachingData.PersonalniTreners.Add(el);
                        
                    }
                }
            }
            await _sessionDataService.SetSessionDataAsync(_session,_sessionData);
            return Redirect("/AdministratorPage?culture=" +id1.Name);
        }

        public async Task<IActionResult> OnPostMoreC(CultureInfo id1)
        {
            _cachingData.Btn = 0;
            var res = await _databaseService.GetClanove(null,++_cachingData.CC);
            if (res.Status)
            {
                foreach (var el in res.Data as List<Clan>)
                {
                    if (_cachingData.Clans.Find(x => x.ID == el.ID) == null)
                    {
                        _cachingData.Clans.Add(el);
                        
                    }
                }
            }
            await _sessionDataService.SetSessionDataAsync(_session,_sessionData);
            return Redirect("/AdministratorPage?culture=" +id1.Name);
        }
        public async Task<IActionResult> OnPostMorePT(CultureInfo id1)
        {
            _cachingData.Btn = 2;
            var res = await _databaseService.GetPersonalneTrenere(null,++_cachingData.CPT);
            if (res.Status)
            {
                foreach (var el in res.Data as List<PersonalniTrener>)
                {
                    if (_cachingData.PersonalniTreners.Find(x => x.ID == el.ID) == null)
                    {
                        _cachingData.PersonalniTreners.Add(el);
                    }
                }
            }
            await _sessionDataService.SetSessionDataAsync(_session,_sessionData);
            return Redirect("/AdministratorPage?culture=" +id1.Name);
        }
        public async Task<IActionResult> OnPost()
        {
            if (SelectedCalculation != null)
            {
                if (SelectedCalculation == 0)
                {
                    if (_clanovi.Count> 0)
                        _cachingData.Clans = _clanovi.OrderBy(x=>x.Ime).ToList();
                    if (_personalniTreneri.Count> 0)
                        _cachingData.PersonalniTreners = _personalniTreneri.OrderBy(x=>x.Ime).ToList();
                }
                if (SelectedCalculation == 1)
                {
                    if (_clanovi.Count> 0)
                        _cachingData.Clans = _clanovi.OrderBy(x=>x.Prezime).ToList();
                    if (_personalniTreneri.Count> 0)
                        _cachingData.PersonalniTreners = _personalniTreneri.OrderBy(x=>x.Prezime).ToList();
                }
            }
            
            var requestCultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
            var requestCulture = requestCultureFeature.RequestCulture;
            var culture = requestCulture.Culture;
            await _sessionDataService.SetSessionDataAsync(_session,_sessionData);
            return Redirect("/AdministratorPage?culture=" +culture);
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
