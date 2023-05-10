using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Threading.Tasks;
using backend.Models;
using backend.Models.UserAuth;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace backend.Pages
{
    public class KontaktPage : PageModel
    {   private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISessionDataService _sessionDataService;
        private readonly ResourceManager _resourceManager;
        private readonly IDatabaseService _databaseService;
        [BindRequired] [BindProperty] public string Ime { get; set; }
        [BindRequired] [BindProperty] public string Prezime { get; set; }
        [BindRequired] [BindProperty] public Kontakt Con { get; set; }
        public KontaktPage(IHttpContextAccessor httpContextAccessor, ISessionDataService sessionDataService, IDatabaseService databaseService)
        {
            _httpContextAccessor = httpContextAccessor;
            _sessionDataService = sessionDataService;
            _databaseService = databaseService;
            _resourceManager = new ResourceManager(typeof(Translations.kontakt));
        }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPost()
        {
            var _userSessionData = _sessionDataService.GetSessionData(_httpContextAccessor.HttpContext.Session);

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
            else
            {
                Con.Ime = Ime;
                Con.Prezime = Prezime;
            
                var res = await _databaseService.PostKomentar(Con);
                if (res.Status)
                {
                    return RedirectToPage("/kontaktpage");
                }
            }
            return Page();
        }

        private void FormErorrs()
        {
            foreach (var modelStateKey in ViewData.ModelState.Keys)
            {
                var value = ViewData.ModelState[modelStateKey];
                if (value.Errors.Count != 0)
                {
                    string keyInResource = null;
                    switch (modelStateKey)
                    {
                        case "Con.Email":
                            ViewData.ModelState.GetValueOrDefault("Con.Email").Errors.Insert(0,
                                new ModelError(_resourceManager.GetString("val-email", CultureInfo.CurrentCulture)));
                            break;
                        case "Con.Poruka":
                            value.Errors.Insert(0,
                                new ModelError(string.Format(
                                    _resourceManager.GetString("val-required", CultureInfo.CurrentCulture)!,
                                    _resourceManager.GetString("phporuka", CultureInfo.CurrentCulture))));
                            break;
                    }
                  
                }
            }
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