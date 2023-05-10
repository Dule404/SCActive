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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace backend.Pages
{
    public class Prijava : PageModel
    {
        private readonly ResourceManager _resourceManager;
        private readonly IDatabaseService _databaseService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISessionDataService _sessionDataService;
        [BindRequired] [BindProperty] public string Email { get; set; }
        [BindRequired] [BindProperty] public string Password { get; set; }

        public Prijava(IDatabaseService databaseService, IHttpContextAccessor httpContextAccessor,
            ISessionDataService sessionDataService)
        {
            _databaseService = databaseService;
            _httpContextAccessor = httpContextAccessor;
            _sessionDataService = sessionDataService;
            _resourceManager = new ResourceManager(typeof(Translations.prijava));
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                try
                {
                    foreach (var modelStateKey in ViewData.ModelState.Keys)
                    {
                        var value = ViewData.ModelState[modelStateKey];
                        string keyInResource = null;
                        var execute = true;
                        switch (modelStateKey)
                        {
                            case "Email":
                                if (!string.IsNullOrEmpty(Email))
                                    execute = false;
                                keyInResource = "email";
                                break;
                            default:
                                if (!string.IsNullOrEmpty(Password))
                                    execute = false;
                                keyInResource = "lozinka";
                                break;
                        }
                        if (execute)
                            value.Errors.Insert(0,
                                new ModelError(string.Format(
                                    _resourceManager.GetString("val-required", CultureInfo.CurrentCulture)!,
                                    _resourceManager.GetString(keyInResource, CultureInfo.CurrentCulture))));
                    }
                }
                catch (Exception e)
                {
                    return Page();
                }

                return Page();
            }

            var resp = await _databaseService.LogUser(Email, Password);
            if (resp.Status)
            {
                var _userSessionData = await _sessionDataService.GetSessionDataAsync(_httpContextAccessor.HttpContext.Session);
                var clanList = (resp.Data.FirstOrDefault() as List<Clan>);
                if (clanList != null)
                {
                    var clan = clanList.First();
                    _userSessionData = new SessionData
                    {
                        Id = clan.ID,
                        Ime = clan.Ime,
                        Prezime = clan.Prezime,
                        Email = clan.Email,
                        Korpa = new List<global::backend.Models.Proizvod>(),
                        TipKorisnika = (UserCategory)clan.Role,
                        Slika = clan.Slika
                    };
                }
                await _sessionDataService.SetSessionDataAsync(_httpContextAccessor.HttpContext.Session,
                    _userSessionData);
                return Redirect("/Index?culture="+CultureInfo.CurrentCulture);
            }
            ViewData.ModelState["Password"].Errors.Insert(0,
                new ModelError(_resourceManager.GetString("val-invalidlog", CultureInfo.CurrentCulture)));
            await Task.CompletedTask;
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