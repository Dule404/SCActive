using System;
using System.Globalization;
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
    public class IndexModel : PageModel
    {
        public readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISessionDataService _sessionDataService;
        private readonly IDatabaseService _databaseService;
        private readonly ITranslatorService _translatorService;
        private ISession _session;
        private SessionData _sessionData;
        private readonly ResourceManager _rm;
        
        [BindProperty]
        public Post Post { get; set; }
        
        

        
        public IndexModel(IHttpContextAccessor httpContextAccessor, ISessionDataService sessionDataService, IDatabaseService databaseService, ITranslatorService translatorService)
        {
            _httpContextAccessor = httpContextAccessor;
            _sessionDataService = sessionDataService;
            _databaseService = databaseService;
            _translatorService = translatorService;
            _session = _httpContextAccessor.HttpContext.Session;
            _sessionData = _sessionDataService.GetSessionData(_session);
            _rm = new ResourceManager(typeof(backend.Translations.index));
        }
        
        public void OnGet()
        {
            _session = _httpContextAccessor.HttpContext.Session;
            _sessionData = _sessionDataService.GetSessionData(_session);
        }



        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                try
                {
                    foreach (var modelStateKey in ViewData.ModelState.Keys)
                    {
                        var value = ViewData.ModelState[modelStateKey];
                        string keyInResource = null;
                        switch (modelStateKey)
                        {
                            case "Post.Message":
                                keyInResource = "invalidPost";
                                break;
                        }

                        if (keyInResource != null)
                            value.Errors.Insert(0, new ModelError(_rm.GetString(keyInResource, CultureInfo.CurrentCulture)));
                    }
                }
                catch 
                {
                    return Page();
                }
                return Page();
            }


            if (_sessionData.TipKorisnika == UserCategory.NotLogged)
                return Page();

            Post.ClanID = _sessionData.Id;
            await _databaseService.PostPost(Post);
            
            return Redirect("/Index?culture=" + CultureInfo.CurrentCulture);
        }
        public async Task<IActionResult> OnPostProizvodHandler(int id1)
        {
            _sessionData.CachingData.proizvodID = id1;
            await _sessionDataService.SetSessionDataAsync(_session, _sessionData);
            return RedirectToPage("/Proizvod");
        }

        public async Task<IActionResult> OnPostTrenerHandler(int id1)
        {
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
