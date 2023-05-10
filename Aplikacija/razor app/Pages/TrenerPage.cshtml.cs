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
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace backend.Pages
{
    public class TrenerPage : PageModel
    {
        private readonly IDatabaseService _databaseService;
        private ISessionDataService _sessionDataService;
        private IHttpContextAccessor _httpContextAccessor;
        private ISession _session;
        private ICachingData _cachingData;
        private SessionData _sessionData;
        [BindProperty]
        public int SelectedCalculation { get; set; }

        public TrenerPage(IDatabaseService databaseService, ISessionDataService sessionDataService, IHttpContextAccessor httpContextAccessor)
        {
            _databaseService = databaseService;
            _sessionDataService = sessionDataService;
            _httpContextAccessor = httpContextAccessor;
            _session = _httpContextAccessor.HttpContext.Session;
            _sessionData = _sessionDataService.GetSessionData(_session);
            _cachingData = _sessionData.CachingData;
            clans = _cachingData.Clans;
            if (_cachingData.PersonalniTreners.Any())
            {
                personalniTreneri = _cachingData.PersonalniTreners.First();
            }
            btnClanovi = _cachingData.Btn;
            zahtevPersonalniTreneri = _cachingData.ZahteviPersonalniTrener;
            if(_cachingData.Poruka != String.Empty)
            {
                poruka = _cachingData.Poruka;
            }
        }

        public Clan clan { get; set; }
        public PersonalniTrener personalniTreneri { get; set; }
        public List<ZahtevPersonalniTrener> zahtevPersonalniTreneri { set; get; }
        public List<Clan> clans { get; set; }
        public UserCategory tipTrenera { get; set; }
        public int btnClanovi { get; set; }
        public string poruka { get; set; }

        
        public async Task<IActionResult> OnGetAsync()
        {  
            _session = _httpContextAccessor.HttpContext.Session;
            _sessionData = _sessionDataService.GetSessionData(_session);
            _cachingData = _sessionData.CachingData;
            clans = _cachingData.Clans;
            if (_cachingData.PersonalniTreners.Any())
            {
                personalniTreneri = _cachingData.PersonalniTreners.First();
            }
            btnClanovi = _cachingData.Btn;
            zahtevPersonalniTreneri = _cachingData.ZahteviPersonalniTrener;
            if(_cachingData.Poruka != String.Empty)
            {
                poruka = _cachingData.Poruka;
            }
           
            try
            {
                
                var user = await _sessionDataService.GetSessionDataAsync(_httpContextAccessor.HttpContext.Session);
                
                if (user.TipKorisnika == UserCategory.PersonalTrainer && personalniTreneri == null)
                {
                    personalniTreneri = new PersonalniTrener();
                    var resp = await _databaseService.GetPersonalneTrenere(new List<int> { user.Id });
                    personalniTreneri = resp.Data.First() as PersonalniTrener;
                    tipTrenera = UserCategory.PersonalTrainer;
                    _cachingData.PersonalniTreners.Add(personalniTreneri);
                }
                else if (tipTrenera == UserCategory.PersonalTrainer)
                {
                    _cachingData.CPT = 1;
                    //var resp1 = await _databaseService.PrikaziClanovePersonalnogTrenera(personalniTreneri.ID);
                    //_cachingService.Clans = resp1.Data as List<Clan>;
                }

            }
            catch (Exception ex)
            {
               
                RedirectToPage("/Prijava");
            }
            await _sessionDataService.SetSessionDataAsync(_session,_sessionData);
            return Page();
        }

        public async Task<IActionResult> OnPostPrihvati(int idc)
        {
            if(personalniTreneri!=null)
            {
                var resp = await _databaseService.PersonalniTrenerPrihvatiZahtev(personalniTreneri.ID, idc);
                if (!resp.Status)
                {
                    _cachingData.Poruka = resp.Message.Single();
                    _cachingData.Btn = 3;
                    System.Diagnostics.Debug.WriteLine("Message-->"+poruka);
                }
                else
                {
                    var resp2=await _databaseService.ObrisiZahteve(idc);
                    List<ZahtevPersonalniTrener> zpt = new List<ZahtevPersonalniTrener>();
                    foreach (var el in _cachingData.ZahteviPersonalniTrener)
                    {
                        if ((el.Clan.ID != idc) && (el.PersonalniTrener.ID != personalniTreneri.ID))
                        {
                            zpt.Add(el);
                        }
                    }
                    _cachingData.ZahteviPersonalniTrener = zpt;
                }
            }
            await _sessionDataService.SetSessionDataAsync(_session,_sessionData);
            return Redirect("/TrenerPage");
        }

        public async Task<IActionResult> OnPostOdbij(int idc)
        {
            if (personalniTreneri != null)
            {
                var resp = await _databaseService.PersonalniTrenerOdbijZahtev(personalniTreneri.ID, idc);
                if (!resp.Status)
                {
                    _cachingData.Poruka = resp.Message.Single();
                    _cachingData.Btn = 3;
                    System.Diagnostics.Debug.WriteLine("Message-->" + poruka);
                }
                else
                {
                    List<ZahtevPersonalniTrener> zpt = new List<ZahtevPersonalniTrener>();

                    foreach (var el in _cachingData.ZahteviPersonalniTrener)
                    {
                        if(el.Clan.ID!=idc)
                        {
                            zpt.Add(el);
                        }
                    }
                    _cachingData.ZahteviPersonalniTrener = zpt;
                }
            }
            await _sessionDataService.SetSessionDataAsync(_session,_sessionData);
            return Redirect("/TrenerPage");
        }
        
        public async Task<IActionResult> OnPostObrisi(int idc)
        {
            try
            {
                var user = await _sessionDataService.GetSessionDataAsync(_httpContextAccessor.HttpContext.Session);

                if (personalniTreneri!=null)
                {
                    var resp = await _databaseService.PersonalniTrenerObrisiClana(personalniTreneri.ID, idc);
                    if(!resp.Status)
                    {
                        _cachingData.Poruka = resp.Message.Single();
                        _cachingData.Btn = 3;
                    }
                    else
                    {
                        List<Clan> cl=new List<Clan>();
                        foreach(var el in _cachingData.Clans)
                        {
                            if(el.ID!=idc)
                            {
                                cl.Add(el);
                            }
                        }
                        _cachingData.Clans=cl;
                    }
                }
            }
            catch(Exception ex)
            {
                _cachingData.Poruka = "Doslo je do greske...";
                _cachingData.Btn = 3;
            }
            await _sessionDataService.SetSessionDataAsync(_session,_sessionData);
            return Redirect("/TrenerPage");
        }
        

        public async Task<IActionResult> OnPostClanovi(CultureInfo id1)
        {
            try
            {
                _cachingData.Btn = 1;

               if(personalniTreneri!=null)
                {
                    var resp = await _databaseService.PrikaziClanovePersonalnogTrenera(personalniTreneri.ID);
                    _cachingData.Clans = resp.Data as List<Clan>;
                    /*if (_cachingService.CC != 0)
                    {
                        return Redirect("/TrenerPage?culture=" + id1.Name);
                    }*/
                }

                if(!_cachingData.Clans.Any())
                {
                    _cachingData.Poruka = "Nemate klijente...";
                    _cachingData.Btn = 6;
                }
            }
            catch (Exception ex)
            {
                _cachingData.Poruka = "Doslo je do greske,pokusajte ponovo...";
                _cachingData.Btn = 3;
                await _sessionDataService.SetSessionDataAsync(_session,_sessionData);
                return Redirect("/TrenerPage?culture=" +id1.Name);
            }
            await _sessionDataService.SetSessionDataAsync(_session,_sessionData);
            return Redirect("/TrenerPage?culture=" +id1.Name);
        }

        public async Task<IActionResult> OnPostZahtevi(CultureInfo id1)
        {
            try
            {
                _cachingData.Btn = 2;
                if (personalniTreneri != null)
                {
                    var resp = await _databaseService.PrikaziZahtevePersonalnogTrenera(personalniTreneri.ID);
                    if(resp.Status)
                    {
                        zahtevPersonalniTreneri = new List<ZahtevPersonalniTrener>();
                        zahtevPersonalniTreneri = resp.Data as List<ZahtevPersonalniTrener>;
                        _cachingData.ZahteviPersonalniTrener = zahtevPersonalniTreneri;
                    }
                    else
                    {
                        _cachingData.Poruka = resp.Message.Single();
                        System.Diagnostics.Debug.WriteLine(resp.Message.Single());
                    }
                }
            }
            catch (Exception ex)
            {
                _cachingData.Poruka = "Doslo je do greske,pokusajte ponovo...";
                _cachingData.Btn = 3;
                await _sessionDataService.SetSessionDataAsync(_session,_sessionData);
                return Redirect("/TrenerPage?culture=" + id1.Name);
            }
            await _sessionDataService.SetSessionDataAsync(_session,_sessionData);
            return Redirect("/TrenerPage?culture=" + id1.Name);
        }
        public async Task<IActionResult> OnPost()
        {
            if (SelectedCalculation != null)
            {
                if (SelectedCalculation == 0)
                {
                    if (clans.Count> 0)
                        _cachingData.Clans = clans.OrderBy(x=>x.Ime).ToList();
                    if (zahtevPersonalniTreneri.Count> 0)
                        _cachingData.ZahteviPersonalniTrener = zahtevPersonalniTreneri.OrderBy(x=>x.Clan.Ime).ToList();
                }
                if (SelectedCalculation == 1)
                {
                    if (clans.Count> 0)
                        _cachingData.Clans = clans.OrderBy(x=>x.Prezime).ToList();
                    if (zahtevPersonalniTreneri.Count> 0)
                        _cachingData.ZahteviPersonalniTrener = zahtevPersonalniTreneri.OrderBy(x=>x.Clan.Prezime).ToList();
                }
            }
            
            var requestCultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
            var requestCulture = requestCultureFeature.RequestCulture;
            var culture = requestCulture.Culture;
            await _sessionDataService.SetSessionDataAsync(_session,_sessionData);
            return Redirect("/TrenerPage?culture=" +culture);
        }

        public async Task<IActionResult> OnPostMoreC(CultureInfo id1)
        {
            _cachingData.Btn = 1;
            if(personalniTreneri!=null)
            {
                var resp1 = await _databaseService.PrikaziClanovePersonalnogTrenera(personalniTreneri.ID, ++_cachingData.CC);

                if (resp1.Status)
                {
                    if (resp1.Status)
                    {
                        List<Clan> lc = new List<Clan>();
                        foreach (var clan in resp1.Data as List<Clan>)
                        {
                            if (_cachingData.Clans.Find(x => x.ID == clan.ID) == null)
                            {
                                lc.Add(clan);
                            }
                        }
                        _cachingData.Clans = lc;
                    }
                }
            }
            await _sessionDataService.SetSessionDataAsync(_session,_sessionData);
            return Redirect("/TrenerPage?culture=" + id1.Name);
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