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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using backend.Models.DbResponse;
using ModelError = Microsoft.AspNetCore.Mvc.ModelBinding.ModelError;

namespace backend.Pages
{
    
    public class KorisnikEditModel : PageModel
    {
        private readonly ResourceManager _resourceManager;
        private readonly IDatabaseService _databaseService;
        private readonly ICloudStorage _cloudStorage;
        private ISessionDataService _sessionDataService;
        private IHttpContextAccessor _httpContextAccessor;
        private ISession _session;
        private SessionData _sessionData;
        public ICachingData CachingData;

        public KorisnikEditModel(IDatabaseService databaseService, ISessionDataService sessionDataService, IHttpContextAccessor httpContextAccessor, ICloudStorage cloudStorage)
        {
            _databaseService = databaseService;
            _cloudStorage = cloudStorage;
            _sessionDataService = sessionDataService;
            _httpContextAccessor = httpContextAccessor;
            _resourceManager = new ResourceManager(typeof(Translations.korisnikedit));
            _session = _httpContextAccessor.HttpContext.Session;
            _sessionData = _sessionDataService.GetSessionData(_session);
            CachingData = _sessionData.CachingData;
            btn = CachingData.Btn;
            if (CachingData.Clans.Any())
            {
                _clan = CachingData.Clans.First();
            }
        }

        [BindProperty]
        public Clan Clan { get; set; }
        public Clan _clan { get; set; }
        public PersonalniTrener _personalniTrener { get; set; }
        public Administrator _admin { get; set; }
        public int btn { get; set; }
        public UserCategory UserCategory { get; set; }
        [BindProperty]
        public IFormFile SlikaUpload { get; set; }
        public string Slika { get; set; }

        private void FormErorrs()
        {
            if (ViewData.ModelState.GetValueOrDefault("Clan.Lozinka").Errors.FirstOrDefault() != null)
            {
                ViewData.ModelState.GetValueOrDefault("Clan.Lozinka").Errors.Insert(0,
                    new ModelError(_resourceManager.GetString("val-password", CultureInfo.CurrentCulture)));
            }

            if (ViewData.ModelState.GetValueOrDefault("Clan.Email").Errors.FirstOrDefault() != null)
            {

                ViewData.ModelState.GetValueOrDefault("Clan.Email").Errors.Insert(0,
                    new ModelError(_resourceManager.GetString("val-email", CultureInfo.CurrentCulture)));
            }

            foreach (var modelStateKey in ViewData.ModelState.Keys)
            {
                var value = ViewData.ModelState[modelStateKey];
                var execute = true;
                if (value.Errors.Count != 0)
                {
                    string keyInResource = null;
                    switch (modelStateKey)
                    {
                        case "Clan.Ime":
                            keyInResource = "ime";
                            break;
                        case "Clan.Prezime":
                            keyInResource = "prezime";
                            break;
                        case "Clan.Email":
                            execute = false;
                            keyInResource = "email";
                            break;
                        case "Clan.Lozinka":
                            execute = false;
                            break;
                        case "Sport.Ime":
                            keyInResource = "lblsport";
                            break;
                        default:
                            keyInResource = "datum";
                            break;
                    }

                    if (execute)
                        value.Errors.Insert(0,
                            new ModelError(string.Format(
                                _resourceManager.GetString("val-required", CultureInfo.CurrentCulture)!,
                                _resourceManager.GetString(keyInResource, CultureInfo.CurrentCulture))));
                }
            }
        }
        public async Task<IActionResult> OnGet()
        {
            _session = _httpContextAccessor.HttpContext.Session;
            _sessionData = _sessionDataService.GetSessionData(_session);
            CachingData = _sessionData.CachingData;
            btn = CachingData.Btn;
            if(CachingData.Clans.Any())
                _clan = CachingData.Clans.First();

            try
            {
                var user = await _sessionDataService.GetSessionDataAsync(_httpContextAccessor.HttpContext.Session);

                
                if (user.Id < 0)
                {
                    return RedirectToPage("/Prijava");
                }

                UserCategory = user.TipKorisnika;
                
                if(user.TipKorisnika==UserCategory.Administrator)
                {
                    var resp = await _databaseService.GetAdministrator(new List<int> { user.Id });
                    _admin = resp.Data.First() as Administrator;
                    Slika = _admin.Slika;
                }
                if(user.TipKorisnika==UserCategory.DefaultUser || user.TipKorisnika == UserCategory.PersonalTrainer)
                {
                    var resp = await _databaseService.GetClanove(new List<int> { user.Id});
                    if(resp.Status && resp!=null)
                    {
                        _clan=resp.Data.First() as Clan;
                        CachingData.Clans.Clear();
                        CachingData.Clans.Add(_clan);
                        Slika = _clan.Slika;
                    }
                }
            }
            catch (Exception ex)
            {
                RedirectToPage("/Prijava");
            }

            await _sessionDataService.SetSessionDataAsync(_session, _sessionData);
            return Page();
        }

        public async Task<IActionResult> OnPostPodaci()
        {
            var requestCultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
            var requestCulture = requestCultureFeature.RequestCulture;
            var culture = requestCulture.Culture;
            var user = await _sessionDataService.GetSessionDataAsync(_httpContextAccessor.HttpContext.Session);
            UserCategory = user.TipKorisnika;


            Clan.ID = user.Id;
            SessionData _userSessionData;
            DbResponse resp;
            if (UserCategory == UserCategory.DefaultUser || UserCategory == UserCategory.PersonalTrainer)
            {
                resp = await _databaseService.UpdateClan(Clan);
            }
            else
            {
                Administrator admin = new Administrator(Clan);
                admin.ID = Clan.ID;
                admin.Email = Clan.Email;
                resp = await _databaseService.UpdateAdmin(admin);
            }
            if (resp.Status)
            {

                _userSessionData =
                    await _sessionDataService.GetSessionDataAsync(_httpContextAccessor.HttpContext.Session);
                _userSessionData.Ime = Clan.Ime;
                _userSessionData.Prezime = Clan.Prezime;
                _userSessionData.Email = Clan.Email;
                await _sessionDataService.SetSessionDataAsync(_httpContextAccessor.HttpContext.Session,
                    _userSessionData);
                return Redirect("/Korisnik?culture=" + culture);
            }

                ViewData.ModelState["Clan.Email"].Errors.Insert(0,
                    new ModelError(_resourceManager.GetString("val-EmailRegistered", CultureInfo.CurrentCulture)));
                await Task.CompletedTask;
                return Page();
        }

        public async Task<IActionResult> OnPostMail()
        {
            var _userSessionData = _sessionDataService.GetSessionData(_httpContextAccessor.HttpContext.Session);
            if (!ModelState.IsValid)
            {
                try
                {
                    if (Clan.Email != null)
                    {
                        if (ViewData.ModelState.GetValueOrDefault("Clan.Email").Errors.FirstOrDefault() != null)
                        {
                            CachingData.Btn = 6;
                            ViewData.ModelState.GetValueOrDefault("Clan.Email").Errors.Insert(0,
                                new ModelError(_resourceManager.GetString("val-email", CultureInfo.CurrentCulture)));

                            await _sessionDataService.SetSessionDataAsync(_session, _sessionData);
                            return Redirect("/KorisnikEdit?culture=" + CultureInfo.CurrentCulture);
                        }
                    }
                }
                catch(Exception ex)
                {
                    return RedirectToPage("/KorisnikEdit");
                }   
            }

            var requestCultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
            var requestCulture = requestCultureFeature.RequestCulture;
            var culture = requestCulture.Culture;
            var user = await _sessionDataService.GetSessionDataAsync(_httpContextAccessor.HttpContext.Session);
            UserCategory = user.TipKorisnika;

            Clan.ID = user.Id;
            DbResponse resp = await _databaseService.GetClanByEmail(Clan.Email);
            if (resp.Status)
            {
                ViewData.ModelState["Clan.Email"].Errors.Insert(0,
                    new ModelError(_resourceManager.GetString("val-EmailRegistered", CultureInfo.CurrentCulture)));
                CachingData.Btn = 7;
                await Task.CompletedTask;
                await _sessionDataService.SetSessionDataAsync(_session, _sessionData);
                return Redirect("/KorisnikEdit?culture=" + CultureInfo.CurrentCulture);
            }

            if(UserCategory==UserCategory.DefaultUser || UserCategory== UserCategory.PersonalTrainer)
            {
                resp = await _databaseService.UpdateClan(Clan);
            }
            else
            {
                Administrator admin = new Administrator(Clan);
                admin.ID = Clan.ID;
                admin.Email = Clan.Email;
                resp = await _databaseService.UpdateAdmin(admin);
            }

            if (resp.Status)
            {

                _userSessionData =
                    await _sessionDataService.GetSessionDataAsync(_httpContextAccessor.HttpContext.Session);
                _userSessionData.Email = Clan.Email;
                await _sessionDataService.SetSessionDataAsync(_httpContextAccessor.HttpContext.Session,
                    _userSessionData);
                return Redirect("/Korisnik?culture=" + culture);
            }
            await _sessionDataService.SetSessionDataAsync(_session, _sessionData);
            return Redirect("/KorisnikEdit?culture=" + CultureInfo.CurrentCulture);
        }

        public async Task<IActionResult> OnPostPassword()
        {
            var _userSessionData = _sessionDataService.GetSessionData(_httpContextAccessor.HttpContext.Session);
            if (!ModelState.IsValid)
            {
                try
                {
                    if (Clan.Lozinka != null)
                    {
                        if (ViewData.ModelState.GetValueOrDefault("Clan.Lozinka").Errors.FirstOrDefault() != null)
                        {
                            CachingData.Btn = 9;
                            ViewData.ModelState.GetValueOrDefault("Clan.Lozinka").Errors.Insert(0,
                                new ModelError(_resourceManager.GetString("val-password", CultureInfo.CurrentCulture)));
                            await _sessionDataService.SetSessionDataAsync(_session, _sessionData);
                            return Redirect("/KorisnikEdit?culture=" + CultureInfo.CurrentCulture);
                        }
                    }
                }
                catch(Exception ex)
                {
                    return Redirect("/KorisnikEdit?culture=" + CultureInfo.CurrentCulture);
                }
            }

            var requestCultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
            var requestCulture = requestCultureFeature.RequestCulture;
            var culture = requestCulture.Culture;
            var user = await _sessionDataService.GetSessionDataAsync(_httpContextAccessor.HttpContext.Session);
            UserCategory = user.TipKorisnika;

            Clan.ID = user.Id;
            DbResponse resp;
            if (UserCategory == UserCategory.DefaultUser || UserCategory == UserCategory.PersonalTrainer)
            {
                resp = await _databaseService.UpdateClan(Clan);
            }
            else
            {
                Administrator admin = new Administrator(Clan);
                admin.ID = Clan.ID;
                admin.Lozinka = Clan.Lozinka;
                resp = await _databaseService.UpdateAdmin(admin);
            }

            if (resp.Status)
            {
                _userSessionData =
                    await _sessionDataService.GetSessionDataAsync(_httpContextAccessor.HttpContext.Session);
                await _sessionDataService.SetSessionDataAsync(_httpContextAccessor.HttpContext.Session,
                    _userSessionData);
                return Redirect("/Korisnik?culture=" + culture);
            }

            CachingData.Btn = 9;
            await _sessionDataService.SetSessionDataAsync(_session, _sessionData);
            return Redirect("/KorisnikEdit?culture=" + CultureInfo.CurrentCulture);
        }

        public async Task<IActionResult> OnPostSlika()
        {
            var user = await _sessionDataService.GetSessionDataAsync(_httpContextAccessor.HttpContext.Session);
            try
            {
                if(SlikaUpload != null && SlikaUpload.Length > 0)
                {
                    if(Slika!=null)
                    {
                        await _cloudStorage.DeleteImage(_clan.Slika);
                    }
                    var urlSlike = await _cloudStorage.UploadAsync(SlikaUpload);
                    if (urlSlike != null)
                    {
                        var res = await _databaseService.AddPicToUser(urlSlike, user.Id);
                        if (res.Status)
                        {
                            _sessionData.Slika = urlSlike;
                        }
                        else
                        {
                            CachingData.Btn = 5;
                        }
                    }
                }
                else
                {
                    CachingData.Btn = 5;//slika nije uploadovana!
                    await _sessionDataService.SetSessionDataAsync(_session, _sessionData);
                    return Redirect("/KorisnikEdit?culture=" + CultureInfo.CurrentCulture);
                }
            }
            catch(Exception ex)
            {
                return Redirect("/KorisnikEdit?culture=" + CultureInfo.CurrentCulture);
            }
            await _sessionDataService.SetSessionDataAsync(_httpContextAccessor.HttpContext.Session,_sessionData);
            return Redirect("/Korisnik?culture=" + CultureInfo.CurrentCulture);
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
