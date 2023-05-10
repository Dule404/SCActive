using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using backend.Enums;
using backend.Models;
using backend.Models.DbResponse;
using backend.Models.UserAuth;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModelError = Microsoft.AspNetCore.Mvc.ModelBinding.ModelError;


namespace backend.Pages
{
    [BindProperties]
    public class Registracija : PageModel
    {
        private readonly ResourceManager _resourceManager;
        private readonly IDatabaseService _databaseService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISessionDataService _sessionDataService;
        public Clan Clan { get; set; }
        public string UserType { get; set; }
        public Sport Sport { get; set; }

        public Registracija(IDatabaseService databaseService, IHttpContextAccessor httpContextAccessor, ISessionDataService sessionDataService)
        {
            _databaseService = databaseService;
            _httpContextAccessor = httpContextAccessor;
            _sessionDataService = sessionDataService;
            _resourceManager = new ResourceManager(typeof(Translations.registracija));
        }
        
        private void FormErorrs()
        {
            if (ViewData.ModelState.GetValueOrDefault("Clan.Lozinka").Errors.FirstOrDefault() !=null)
            {
                ViewData.ModelState.GetValueOrDefault("Clan.Lozinka").Errors.Insert(0,
                    new ModelError(_resourceManager.GetString("val-password", CultureInfo.CurrentCulture)));
            }

            if (ViewData.ModelState.GetValueOrDefault("Clan.Email").Errors.FirstOrDefault() !=null)
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

        public void OnGet()
        {
            
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (UserType == "Korisnik" || UserType == "Default user")
            {
                ModelState.Remove("Sport.Ime");
                ModelState.Remove("Clan.Sport");
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    FormErorrs();
                }
                catch
                {
                    return Page();
                }

                return Page();
            }


            SessionData _userSessionData;
            DbResponse resp;
            if (UserType == "Korisnik" || UserType == "Default user")
            {
                resp = await _databaseService.GetClanByEmail(Clan.Email);
                if (!resp.Status)
                {
                    resp = await _databaseService.PostClan(Clan);
                    if (resp.Status)
                    {

                        _userSessionData =
                            await _sessionDataService.GetSessionDataAsync(_httpContextAccessor.HttpContext.Session);
                        _userSessionData.Id = (int)resp.Data.First();
                        _userSessionData.Email = Clan.Email;
                        _userSessionData.Ime = Clan.Ime;
                        _userSessionData.Prezime = Clan.Prezime;
                        _userSessionData.TipKorisnika = UserCategory.DefaultUser;
                        await _sessionDataService.SetSessionDataAsync(_httpContextAccessor.HttpContext.Session,
                            _userSessionData);
                        return RedirectToPage("/Index");
                    }
                }

                ViewData.ModelState["Clan.Email"].Errors.Insert(0,
                    new ModelError(_resourceManager.GetString("val-EmailRegistered", CultureInfo.CurrentCulture)));
                await Task.CompletedTask;
                return Page();
            }

            resp = await _databaseService.PostSport(Sport);
            if (resp.Status)
            {
                Sport.ID = (int)resp.Data.First();
            }
            else
            {
                if (resp.Data == null)
                    return Page();
                Sport = resp.Data.First() as Sport;
            }

            if (UserType == "Personalni trener" || UserType == "Personal trainer")
            {
                resp = await _databaseService.GetClanByEmail(Clan.Email);
                if (!resp.Status)
                {
                    PersonalniTrener trener = new PersonalniTrener()
                    {
                        Ime = Clan.Ime,
                        Prezime = Clan.Prezime,
                        Email = Clan.Email,
                        Lozinka = Clan.Lozinka,
                        DatumRodjenja = Clan.DatumRodjenja,
                        Telefon = Clan.Telefon,
                        Sport = Sport
                    };
                    resp = await _databaseService.PostPersonalniTrener(trener);
                    if (resp.Status)
                    {
                        _userSessionData =
                            await _sessionDataService.GetSessionDataAsync(_httpContextAccessor.HttpContext.Session);
                        _userSessionData.Id = (int)resp.Data.First();
                        _userSessionData.Email = Clan.Email;
                        _userSessionData.Ime = Clan.Ime;
                        _userSessionData.Prezime = Clan.Prezime;
                        _userSessionData.TipKorisnika = UserCategory.PersonalTrainer;
                        await _sessionDataService.SetSessionDataAsync(_httpContextAccessor.HttpContext.Session,
                            _userSessionData);
                        return RedirectToPage("/Index");
                    }
                }

                ViewData.ModelState["Clan.Email"].Errors.Insert(0,
                    new ModelError(_resourceManager.GetString("val-EmailRegistered", CultureInfo.CurrentCulture)));
                await Task.CompletedTask;

                return Page();
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